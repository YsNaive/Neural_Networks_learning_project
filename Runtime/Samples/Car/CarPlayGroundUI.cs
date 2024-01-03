using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class CarPlayGroundUI : MonoBehaviour
{
    public UIDocument UID;
    public CarController CarController;
    VisualElement root;
    VisualElement infoContainer;
    VisualElement toolsContainer;
    public TextAsset Data4D, Data6D;
    public List<FloatVector> Data4D_x = new(), Data4D_y = new(), Data6D_x = new(), Data6D_y = new();
    void Start()
    {
        foreach (var str in Data4D.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var datas = str.Split(' ');
            FloatVector x = new FloatVector(4);
            FloatVector y = new FloatVector(1);
            x[0] = -1;
            x[1] = float.Parse(datas[0]);
            x[2] = float.Parse(datas[1]);
            x[3] = float.Parse(datas[2]);
            y[0] = float.Parse(datas[3])+40;
            Data4D_x.Add(x);
            Data4D_y.Add(y);
        }
        foreach (var str in Data6D.text.Split(new char[] { '\n', '\r' },StringSplitOptions.RemoveEmptyEntries))
        {
            var datas = str.Split(' ');
            FloatVector x = new FloatVector(6);
            FloatVector y = new FloatVector(1);
            x[0] = -1;
            x[1] = float.Parse(datas[0]);
            x[2] = float.Parse(datas[1]);
            x[3] = float.Parse(datas[2]);
            x[4] = float.Parse(datas[3]);
            x[5] = float.Parse(datas[4]);
            y[0] = float.Parse(datas[5])+40;
            Data6D_x.Add(x);
            Data6D_y.Add(y);
        }
        root = UID.rootVisualElement;
        initCarInfo();
        initTools();
        modifyWarnning = DocRuntime.CreateDocVisual(new DocComponent()
        {
            VisualID = "2",
            TextData = new List<string> { "Model property has been modify.\nRegenerate the model to apply changed." },
            JsonData = JsonUtility.ToJson(new DocDescription.Data() { Type = DocDescription.Type.Danger })
        });

    }
    DocVisual modifyWarnning;
    private void initTools()
    {
        toolsContainer = new VisualElement();
        toolsContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;
        toolsContainer.style.position = Position.Absolute;
        toolsContainer.style.width = Length.Percent(30);
        toolsContainer.style.height = Length.Percent(90);
        toolsContainer.style.left = StyleKeyword.Auto;
        toolsContainer.style.right = Length.Percent(2);
        toolsContainer.style.top = Length.Percent(5);
        toolsContainer.style.SetIS_Style(new ISBorder(DocStyle.Current.SubBackgroundColor, 5));
        toolsContainer.style.SetIS_Style(ISPadding.Pixel(10));
        
        DocStyle.Current.BeginLabelWidth(ISLength.Pixel(160));
        var choices = new List<string>() { "4D", "6D" };
        var dropdown = DocRuntime.NewDropdown("Train Data", choices, str => { toolsContainer.Add(modifyWarnning); });
        dropdown.Index = 0;
        var oldLayersStr = "4,4";
        int[] layers = new int[] {3, 4, 4, 1 };
        var hiddenLayers = DocRuntime.NewTextField("Hid Layers");
        hiddenLayers.RegisterValueChangedCallback(evt => { if(evt.newValue != oldLayersStr) toolsContainer.Add(modifyWarnning); });
        hiddenLayers.RegisterCallback<FocusOutEvent>(evt =>
        {
            if (hiddenLayers.value == oldLayersStr) return;
            bool isAllow = true;
            foreach (var str in hiddenLayers.value.Split(','))
            {
                if (!int.TryParse(str, out _))
                {
                    isAllow = false;
                    break;
                }
            }
            if (isAllow)
            {
                var strs = hiddenLayers.value.Split(',');
                layers = new int[strs.Length+2];
                layers[0] = (dropdown.Index == 0) ? 3 : 5;
                layers[^1] = 1;
                for (int i=0;i<strs.Length;i++)
                    layers[i+1] = int.Parse(strs[i]);
                toolsContainer.Add(modifyWarnning);
                oldLayersStr = hiddenLayers.value;
            }
            else
            {
                hiddenLayers.value = oldLayersStr;
                if(toolsContainer.Contains(modifyWarnning))
                    toolsContainer.Remove(modifyWarnning);
            }
        });
        hiddenLayers.value = oldLayersStr;

        CarController.Trainer = new CarController.CarDNN.Trainer(CarController.Brain);
        CarController.Trainer.LearningRate = 0.0000025f;
        CarController.Trainer.Epochs = 50;
        var trainVisualContainer = new VisualElement();
        DocStyle.Current.BeginLabelWidth(ISLength.Pixel(200));
        var trainerVisual = CarController.Trainer.CreateEditVisual();
        DocStyle.Current.EndLabelWidth();
        trainVisualContainer.Add(trainerVisual);
        var regenButton = DocRuntime.NewButton("Generate Model", () =>
        {
            if (toolsContainer.Contains(modifyWarnning))
                toolsContainer.Remove(modifyWarnning);
            layers[0] = (dropdown.Index == 0) ? 3 : 5;
            CarController.Brain = new CarController.CarDNN(layers);
            CarController.Trainer = new CarController.CarDNN.Trainer(CarController.Brain);
            CarController.Trainer.LearningRate = 0.0000025f;
            CarController.Trainer.Epochs = 50;
            trainVisualContainer.Clear();
            DocStyle.Current.BeginLabelWidth(ISLength.Pixel(200));
            trainVisualContainer.Add(CarController.Trainer.CreateEditVisual());
            DocStyle.Current.EndLabelWidth();
            CarController.ResetState();
        });
        var loss = DocRuntime.NewTextElement("loss: ");
        var trainBtn = DocRuntime.NewButton("Train", () =>
        {
            var is4D = CarController.Brain.Layers[0].Neurals[0].Weights.Length == CarController.Senser4D.Length;
            var x = is4D ? Data4D_x : Data6D_x;
            var y = is4D ? Data4D_y : Data6D_y;
            CarController.Trainer.Train(x, y);
            loss.text = $"loss: {CarController.Trainer.TrainHistory[^1].Loss}";
        });

        var startBtn = DocRuntime.NewButton("Reset & Run", DocStyle.Current.HintColor, () =>
        {
            CarController.ResetState();
            root.schedule.Execute(CarController.Next).Every(60).Until(() => { return CarController.IsCrash; });
        });
        var resetBtn = DocRuntime.NewButton("Reset", () =>
        {
            CarController.ResetState();
        });
        var stepBtn = DocRuntime.NewButton("Step", () =>
        {
            CarController.Next();
        });
        DocStyle.Current.EndLabelWidth();




        toolsContainer.Add(DocRuntime.NewLabel("Model Settings"));
        toolsContainer.Add(dropdown);
        toolsContainer.Add(hiddenLayers);
        toolsContainer.Add(regenButton);
        regenButton.style.marginBottom = 50;
        toolsContainer.Add(DocRuntime.NewLabel("Model Property"));
        toolsContainer.Add(trainVisualContainer);
        toolsContainer.Add(trainBtn);
        toolsContainer.Add(loss);
        loss.style.marginBottom = 50;
        toolsContainer.Add(DocRuntime.NewLabel("Model Test"));
        toolsContainer.Add(startBtn);
        toolsContainer.Add(resetBtn);
        toolsContainer.Add(stepBtn);

        root.Add(toolsContainer);
    }
    void initCarInfo()
    {
        infoContainer = new VisualElement();
        infoContainer.style.backgroundColor = DocStyle.Current.BackgroundColor;
        infoContainer.style.position = Position.Absolute;
        infoContainer.style.width = Length.Percent(25);
        infoContainer.style.height = Length.Percent(90);
        infoContainer.style.left = Length.Percent(2);
        infoContainer.style.top = Length.Percent(5);
        infoContainer.style.SetIS_Style(new ISBorder(DocStyle.Current.SubBackgroundColor, 5));
        infoContainer.style.SetIS_Style(ISPadding.Pixel(10));

        DocStyle.Current.BeginLabelWidth(ISLength.Pixel(120));
        infoContainer.Add(DocRuntime.NewLabel("Car Property"));
        var position = RuntimeDrawer.CreateDrawer("Position",new Vector2(CarController.Senser6D[1], CarController.Senser6D[2]));
        var forward = RuntimeDrawer.CreateDrawer("Forward",CarController.Senser4D[1]);
        var right = RuntimeDrawer.CreateDrawer("Right",CarController.Senser4D[2]);
        var left = RuntimeDrawer.CreateDrawer("Left",CarController.Senser4D[3]);
        var steering = RuntimeDrawer.CreateDrawer("Steering", CarController.SteeringDeg);
        DocStyle.Current.EndLabelWidth();
        root.schedule.Execute(() =>
        {
            position.SetValue(new Vector2(CarController.Senser6D[1], CarController.Senser6D[2]));
            forward.SetValue(CarController.Senser4D[1]);
            right.SetValue(CarController.Senser4D[2]);
            left.SetValue(CarController.Senser4D[3]);
            steering.SetValue(CarController.SteeringDeg);
        }).Every(100);

        var saveLogBtn = DocRuntime.NewButton("Save Path", () =>
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Save Path Log", "", "path log", "txt");
            if (path == "") return;
            StringBuilder stringBuilder = new StringBuilder();
            foreach(var log in CarController.PathLog)
            {
                for(int i = 1; i < log.Count-1; i++)
                    stringBuilder.Append(log[i].ToString("n7")).Append(' ');
                stringBuilder.Append(log[^1].ToString("n7")).Append('\n');
            }
            File.WriteAllText(path, stringBuilder.ToString());
        });
        saveLogBtn.style.marginTop = StyleKeyword.Auto;
        infoContainer.Add(position);
        infoContainer.Add(forward);
        infoContainer.Add(right);
        infoContainer.Add(left);
        infoContainer.Add(steering);
        infoContainer.Add(saveLogBtn);

        root.Add(infoContainer);
    }

}
