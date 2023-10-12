using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class OneHotDNNSample : VisualElement
{
    public OneHotDNN Model;
    public OneHotDNN.Trainer Trainer = new OneHotDNN.Trainer(null);

    public OneHotDNNSample()
    {
        Data3DRenderer = Data3DRenderer.Instance;
        this.style.SetIS_Style(ISMargin.Pixel(15));
        Add(DocRuntime.NewLabel("Model"));
        Add(createSettings());
        Add(createTrain());
    }
    MultiDimensionDataReader dataReader;
    VisualElement createSettings()
    {
        VisualElement root = DocRuntime.NewEmpty();
        var dataStates = DocRuntime.NewTextElement("N/A");
        var valSplitDrawer = (FloatDrawer)RuntimeDrawer.CreateDrawer("val split", 0.2f);
        valSplitDrawer.OnValueChanged += () => { if (valSplitDrawer.value > 1f) valSplitDrawer.value = 1f; if (valSplitDrawer.value < 0f) valSplitDrawer.value = 0f; };
        var valSplitDrawerLabel = valSplitDrawer.Q<TextField>().labelElement;
        valSplitDrawerLabel.style.minWidth = 0;
        valSplitDrawerLabel.style.width = StyleKeyword.Auto;

        var selectData = DocRuntime.NewButton("Select Data", DocStyle.Current.HintColor, () =>
        {
            trainBtn.SetEnabled(false);
            var path = StandaloneFileBrowser.OpenFilePanel("Select Data", "", "txt", false);
            if(path.Length == 0)
            {
                dataStates.text = "N/A";
                return;
            }
            dataReader = new MultiDimensionDataReader(File.ReadAllText(path[0]), valSplitDrawer.value, true);
            if (!dataReader.IsSuccess)
            {
                dataStates.text = "Data Format Error";
                return;
            }
            Model = null;
            dataReader.MinMaxNormalize();
            if(dataReader.DimensionX == 4)
            {
                Data3DRenderer.Render(dataReader);
                data3DContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                data3DContainer.style.display = DisplayStyle.None;
            }
            if(dataReader.DimensionX == 3)
            {
                repaintData2D();
                data2DImg.style.display = DisplayStyle.Flex; 
            }
            else
            {
                data2DImg.style.display = DisplayStyle.None;
            }
            int i = path[0].LastIndexOf("\\") + 1;
            dataStates.text = $"<b>{path[0].Substring(i, path[0].Length - i - 4)}</b> | <b>size</b>: {dataReader.DataCount} | <b>dimension</b>: ({dataReader.DimensionX}, {dataReader.DimensionY})\n<b>train/val</b> ({dataReader.Train_x.Count}, {dataReader.Val_x.Count})";
        });
        root.Add(DocRuntime.NewHorizontalBar(1f,dataStates, null, null, valSplitDrawer, selectData));

        var hiddenLayersField = DocRuntime.NewTextField("Hidden Layers");
        hiddenLayersField.labelElement.style.minWidth = 0;
        hiddenLayersField.labelElement.style.width = StyleKeyword.Auto;
        var modelStates = DocRuntime.NewTextElement("N/A");
        Button updateModel = null;
        updateModel = DocRuntime.NewButton("New Model", DocStyle.Current.HintColor, () =>
        {
            if(dataReader == null)
            {
                modelStates.text = "Please Select Data First";
                return;
            }
            var layerStrs = hiddenLayersField.value.Replace(" ","").Split(',');
            List<int> layers = new List<int>();
            if(hiddenLayersField.value != "")
            {
                foreach (var lstr in layerStrs)
                {
                    int num;
                    if (int.TryParse(lstr, out num))
                    {
                        layers.Add(num);
                    }
                    else
                    {
                        modelStates.text = "Hidden Layers Format Error";
                        updateModel.style.backgroundColor = DocStyle.Current.DangerColor;
                        layers.Clear();
                        Model = null;
                        RuntimeWindow.GetWindow<RuntimeInspector>().Target = null;
                        return;
                    }
                }
            }
            layers.Insert(0, dataReader.DimensionX-1);
            layers.Add(dataReader.DimensionY);
            Model = new OneHotDNN(layers.ToArray());
            trainBtn.SetEnabled(true);
            Trainer.Model = Model;
            RuntimeWindow.GetWindow<RuntimeInspector>().Target = Model;
            modelStates.text = "<b>Layers</b>: ";
            foreach (var layer in layers)
                modelStates.text += layer + " ";
        });
        root.Add(DocRuntime.NewHorizontalBar(1f, modelStates, null, hiddenLayersField, null, updateModel));
        return root;
    }
    Data3DRenderer Data3DRenderer;
    VisualElement data3DContainer, data2DImg;
    Button trainBtn;
    VisualElement createTrain()
    {
        VisualElement root = DocRuntime.NewEmpty();

        var historyImg = DocRuntime.NewEmpty();
        var historyPainter = new ModelHistoryPainter(200, 150);
        historyImg.style.width = 200;
        historyImg.style.height = 150;
        historyImg.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
        var imgBar = DocRuntime.NewEmptyHorizontal();
        imgBar.style.SetIS_Style(ISMargin.Pixel(10));
        imgBar.Add(historyImg);

        data2DImg = DocRuntime.NewEmpty();
        data2DImg.style.width = 150;
        data2DImg.style.height = 150;
        data2DImg.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
        data2DImg.style.backgroundImage = dataPrinter2D.Texture;
        data2DImg.style.display = DisplayStyle.None;
        data2DImg.style.marginLeft = 15;
        imgBar.Add(data2DImg);

        var data3DImg = new Image();
        data3DContainer = DocRuntime.NewEmptyHorizontal();
        data3DImg.style.width = 150;
        data3DImg.style.height = 150;
        data3DImg.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
        data3DImg.image = Data3DRenderer.RenderTexture;
        data3DContainer.style.display = DisplayStyle.None;
        data3DContainer.style.marginLeft = 15;
        data3DContainer.Add(data3DImg);
        Transform tf = Data3DRenderer.UnitRoot.transform;
        data3DImg.schedule.Execute(() =>
        {
            if (data3DContainer.style.display == DisplayStyle.None) return;
            tf.Rotate(new Vector3(1, 5, 0));
        }).Every(50);
        imgBar.Add(data3DContainer);

        var trainStates = DocRuntime.NewTextElement("steps\nacc\nloss\nval_acc\nval_loss");
        imgBar.Add(trainStates);

        trainBtn = DocRuntime.NewButton("Start Train", DocStyle.Current.SuccessColor, () =>
        {
            Trainer.Train(dataReader.Train_x, dataReader.Train_y, dataReader.Val_x, dataReader.Val_y);
            historyPainter.DrawHistory(Trainer.TrainHistory);
            historyImg.style.backgroundImage = historyPainter.Texture;
            var lastResult = Trainer.TrainHistory[^1];
            var lastValResult = Trainer.ValHistory[^1];
            trainStates.text = $"steps\t{Trainer.TrainHistory.Count}\nacc\t{lastResult.Acc}\nloss\t{lastResult.Loss}\nval_acc\t{lastValResult.Acc}\nval_loss\t{lastValResult.Loss}";
            repaintData2D();
        });
        trainBtn.SetEnabled(false);
        root.Add(DocRuntime.NewLabel("Train"));
        root.Add(Trainer.CreateEditVisual());
        root.Add(imgBar);
        root.Add(trainBtn);
        return root;
    }

    TexturePainter dataPrinter2D = new TexturePainter(100, 100);
    Color[] colors = new Color[] { Color.red, Color.green, Color.cyan, Color.magenta, Color.blue, new Color(.7f, .7f, .4f) };
    bool repaintData2D()
    {
        if(dataReader == null)return false;
        if(dataReader.DimensionX != 3)return false;
        dataPrinter2D.Max = new Vector2(1.1f, 1.1f);
        dataPrinter2D.Min = new Vector2(-0.1f, -0.1f);
        FloatVector x = new FloatVector(3);
        x[0] = -1;
        float wid = dataReader.Max_x - dataReader.Min_x;

        if (Model != null)
        {
            for (int i = 0; i < 120; i++)
            {
                for (int j = 0; j < 120; j++)
                {
                    x[1] = i / 100f - 0.1f;
                    x[2] = j / 100f - 0.1f;
                    Color c = colors[Model.Predict(x).IndexOfMaxValue()];
                    c.a = .15f;
                    dataPrinter2D.DrawPoint(new Vector2(x[1], x[2]), c);
                }
            }
        }
        else
        {
            dataPrinter2D.Clear(Color.clear);
        }
        for(int i=0,imax = dataReader.DataCount; i < imax; i++)
        {
            dataPrinter2D.DrawPoint(new Vector2(dataReader.Data_x[i][1], dataReader.Data_x[i][2]), colors[dataReader.Data_y[i].IndexOfMaxValue()]);
        }
        data2DImg.style.backgroundImage = dataPrinter2D.Texture;
        return true;
    }

}
