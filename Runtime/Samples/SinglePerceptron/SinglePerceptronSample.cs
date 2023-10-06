using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
public class SinglePerceptronSample : VisualElement
{
    public SinglePerceptron Model;
    List<TextAsset> datas;
    VisualElement dataGraphVisual;
    VisualElement valGraphVisual;
    VisualElement historyGraphVisual;
    VisualElement modelGraphVisual;
    Vector2Int graphSize = new Vector2Int(120,120);
    event Action<TextAsset> onDataChanged;

    List<Vector3> pointDatas = new List<Vector3>(); // x, y, group
    List<float[]> trainDatas = new List<float[]>();
    List<float> labelDatas = new List<float>();
    List<float[]> train_x = new List<float[]>(), val_x = new List<float[]>();
    List<float> train_y = new List<float>(), val_y = new List<float>();

    FloatDrawer defaultLearningRateDrawer;
    IntDrawer epochSettingsDrawer;
    FloatDrawer earlyStopRateDrawer;
    FloatDrawer valSplitRateDrawer;
    TextElement trainInfoVisual;

    public List<float> AccHistory = new List<float>();
    public SinglePerceptronSample(List<TextAsset> datas)
    {
        this.datas = datas;
        Color imgBG = new Color(.9f, .9f, .9f);
        dataGraphVisual = DocRuntime.NewEmpty();
        dataGraphVisual.style.SetIS_Style(ISSize.Pixel(150, 150));
        dataGraphVisual.style.SetIS_Style(ISMargin.Pixel(7));
        dataGraphVisual.style.marginRight = 0;
        modelGraphVisual = DocRuntime.NewEmpty();
        modelGraphVisual.style.flexGrow = 1;
        dataGraphVisual.Add(modelGraphVisual);
        dataGraphVisual.style.backgroundColor = imgBG;
        valGraphVisual = DocRuntime.NewEmpty();
        valGraphVisual.style.SetIS_Style(ISSize.Pixel(150, 150));
        valGraphVisual.style.SetIS_Style(ISMargin.Pixel(7));
        valGraphVisual.style.marginRight = 0;
        valGraphVisual.style.backgroundColor = imgBG;
        historyGraphVisual = DocRuntime.NewEmpty();
        historyGraphVisual.style.SetIS_Style(ISSize.Pixel(150, 150));
        historyGraphVisual.style.SetIS_Style(ISMargin.Pixel(7));
        historyGraphVisual.style.marginRight = 0;
        historyGraphVisual.style.backgroundColor = imgBG;
        var graphInfo = DocRuntime.NewEmpty();
        trainInfoVisual = DocRuntime.NewTextElement("");
        graphInfo.Add(trainInfoVisual);
        var graphs = DocRuntime.NewEmptyHorizontal();
        graphs.Add(dataGraphVisual);
        graphs.Add(valGraphVisual);
        graphs.Add(historyGraphVisual);
        graphs.Add(graphInfo);

        epochSettingsDrawer = (IntDrawer)RuntimeDrawer.CreateDrawer("epochs", 50);
        earlyStopRateDrawer = (FloatDrawer)RuntimeDrawer.CreateDrawer("early stop acc", 1f);
        valSplitRateDrawer = (FloatDrawer)RuntimeDrawer.CreateDrawer("val data split", 0f);
        valSplitRateDrawer.value = 0f;

        onDataChanged += updateDataGraph;
        onDataChanged += (data) =>
        {
            Model = new SinglePerceptron(2);
            Model.Neural.Weights[2] = 1;
            Model.LearningRate = defaultLearningRateDrawer.value;
            RuntimeWindow.GetWindow<RuntimeInspector>().Target = Model;
            modelGraphVisual.style.backgroundImage = null;
            valGraphVisual.style.backgroundImage = null;
            historyGraphVisual.style.backgroundImage = null;
            AccHistory.Clear();
        };

        defaultLearningRateDrawer = (FloatDrawer)RuntimeDrawer.CreateDrawer("Default LR", 0.001f);
        defaultLearningRateDrawer.value = 0.001f;

        Button trainBtn = null;
        trainBtn = DocRuntime.NewButton("Start Training",DocStyle.Current.SuccessColor, () =>
        {
            trainBtn.text = "Training...";
            trainBtn.SetEnabled(false);
            bool endTrain = false;
            int ep = 0;

            schedule.Execute(() => // Train
            {
                Model.Train(train_x, train_y);
                var acc = Model.Eval(trainDatas, labelDatas);
                AccHistory.Add(acc);
                if (acc >= earlyStopRateDrawer.value)
                    endTrain = true;
                updateModelGraph();
                trainInfoVisual.text = $"epochs {ep+1}/{epochSettingsDrawer.value}\n\ntotal acc rate:\n   {acc}\n\ntrain acc rate:\n   {Model.Eval(train_x, train_y)}\n\nval acc rate:\n   {Model.Eval(val_x, val_y)}";
                ep++;
                if (ep >= epochSettingsDrawer.value) endTrain = true;
                if (endTrain)
                {
                    trainBtn.text = "Start Training";
                    trainBtn.SetEnabled(true);
                }
            }).Every(75).Until(() => { return endTrain; });
        });
        trainBtn.style.marginTop = 7;

        Add(DocRuntime.NewLabel("SinglePerceptron"));
        Add(selectData());
        Add(defaultLearningRateDrawer);
        Add(valSplitRateDrawer);
        Add(graphs);
        Add(DocRuntime.NewTextElement("Train Settings"));
        Add(epochSettingsDrawer);
        Add(earlyStopRateDrawer);
        Add(trainBtn);
    }

    Color[] colors = new Color[] { Color.magenta, Color.blue, Color.magenta };
    void updateDataGraph(TextAsset data)
    {
        if (data == null) return;
        DataDrawer2D.NewTexture(graphSize.x, graphSize.y);
        DataDrawer2D.Clear(Color.clear);
        pointDatas.Clear();
        Rect bound = new Rect(0, 0, 0, 0);
        foreach (var item in data.text.Split('\n'))
        {
            var vals = item.Split(" ");
            if (vals.Length != 3) continue;
            var p = new Vector3(float.Parse(vals[0]), float.Parse(vals[1]), int.Parse(vals[2]));
            pointDatas.Add(p);
            if (bound.xMin > p.x) bound.xMin = p.x;
            if (bound.yMin > p.y) bound.yMin = p.y;
            if (bound.xMax < p.x) bound.xMax = p.x;
            if (bound.yMax < p.y) bound.yMax = p.y;
        }
        trainDatas.Clear();
        labelDatas.Clear();
        foreach (var p in pointDatas)
        {
            trainDatas.Add(new float[] { -1f, p.x, p.y });
            labelDatas.Add((p.z == 1) ? 1 : -1);
        } 

        Vector2 space = new Vector2((bound.xMax - bound.xMin) / 10f, (bound.yMax - bound.yMin) / 10f);
        bound.xMin -= space.x;
        bound.xMax += space.x;
        bound.yMin -= space.y;
        bound.yMax += space.y;
        if (bound.width < bound.height)
        {
            bound.x -= (bound.height - bound.width) / 2f;
            bound.width = bound.height;
        }
        else
        {
            bound.y -= (bound.width - bound.height) / 2f;
            bound.height = bound.width;
        }
        DataDrawer2D.SetXRange(bound.xMin, bound.xMax);
        DataDrawer2D.SetYRange(bound.yMin, bound.yMax);

        foreach (var p in pointDatas)
        {
            DataDrawer2D.Color = colors[(int)p.z];
            DataDrawer2D.DrawPoints((Vector2)p);
        }

        dataGraphVisual.style.backgroundImage = new StyleBackground(DataDrawer2D.Buffer);

        train_x.Clear();
        train_y.Clear();
        val_x.Clear();
        val_y.Clear();
        for (int i = 0, imax = trainDatas.Count; i < imax; i++)
        {
            if (UnityEngine.Random.Range(0f, 1f) < valSplitRateDrawer.value)
            {
                val_x.Add(trainDatas[i]);
                val_y.Add(labelDatas[i]);
            }
            else
            {
                train_x.Add(trainDatas[i]);
                train_y.Add(labelDatas[i]);
            }
        }
    }
    void updateModelGraph()
    {
        DataDrawer2D.NewTexture(graphSize.x, graphSize.y);
        DataDrawer2D.Clear(Color.clear);
        DataDrawer2D.Color = Color.black;
        Vector2 lastPoint = new Vector2((0 / (float)graphSize.x) * (DataDrawer2D.XMax - DataDrawer2D.XMin) + DataDrawer2D.XMin,0);
        lastPoint.y = ((-lastPoint.x * Model.Neural.Weights[1]) + Model.Neural.Bias) / Model.Neural.Weights[2];
        for (int x = 0; x < graphSize.x; x++)
        {
            Vector2 newPoint = new Vector2((x / (float)graphSize.x) * (DataDrawer2D.XMax - DataDrawer2D.XMin) + DataDrawer2D.XMin, 0);
            newPoint.y = ((-newPoint.x * Model.Neural.Weights[1]) + Model.Neural.Bias) / Model.Neural.Weights[2];
            DataDrawer2D.DrawLine(lastPoint,newPoint);
            lastPoint = newPoint;
        }
        modelGraphVisual.style.backgroundImage = new StyleBackground(DataDrawer2D.Buffer);


        DataDrawer2D.NewTexture(graphSize.x, graphSize.y);
        DataDrawer2D.Clear(Color.clear);
        int i = 0;
        foreach(var input in trainDatas)
        {
            DataDrawer2D.Color = (Model.Predict(input) == labelDatas[i++])? Color.green: Color.red;
            DataDrawer2D.DrawPoints(new Vector2(input[1], input[2]));
        }
        valGraphVisual.style.backgroundImage = new StyleBackground(DataDrawer2D.Buffer);

        DataDrawer2D.NewTexture(graphSize.x, graphSize.y);
        DataDrawer2D.Clear(Color.clear);
        DataDrawer2D.Color = Color.gray;
        int historyCount = AccHistory.Count;
        lastPoint = new Vector2(DataDrawer2D.XMin, DataDrawer2D.YMin);
        for(i=0;i< historyCount; i++)
        {
            Vector2 newPoint = new Vector2(((i+1)/(float)historyCount) * (DataDrawer2D.XMax - DataDrawer2D.XMin) + DataDrawer2D.XMin, 
                                            AccHistory[i]* (DataDrawer2D.YMax - DataDrawer2D.YMin) + DataDrawer2D.YMin);
            DataDrawer2D.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }
        historyGraphVisual.style.backgroundImage = new StyleBackground(DataDrawer2D.Buffer);
    }

    ObjRefDropdown selectData()
    {
        ObjRefDropdown dropdown = new ObjRefDropdown("Select Data");
        List<Object> choices = new List<Object>();
        foreach (var item in datas) {choices.Add(item);} 
        dropdown.Choices = choices;
        dropdown.OnValueChanged += (val) =>
        {
            onDataChanged?.Invoke((TextAsset)val);
        };
        dropdown.Index = 0;
        return dropdown;
    }
}