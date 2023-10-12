using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    UIDocument UID;
    public VisualElement Root;
    public ScrollView ModeContainer;
    float lastGcTime = 0;
    private void Update()
    {
        if((Time.realtimeSinceStartup - lastGcTime) > 60)
        {
            lastGcTime = Time.realtimeSinceStartup;
            GC.Collect();
        }
    }
    void Start()
    {
        Root = DocRuntime.NewEmpty();
        GetComponent<UIDocument>().rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
        GetComponent<UIDocument>().rootVisualElement.Add(Root);
        Root.style.height = Length.Percent(100);
        Root.style.width = Length.Percent(65);

        var inspector = RuntimeWindow.GetWindow<RuntimeInspector>();
        inspector.Dragable = false;
        inspector.Resizeable = false;
        inspector.EnableRightClickMenu = false;
        inspector.Toolbar.Remove(inspector.Toolbar.Q<Button>());
        inspector.LayoutPercent = new Rect(67, 0, 33, 100);

        ModeContainer = DocRuntime.NewScrollView();

        Root.Add(selectModeDropdown());
        Root.Add(ModeContainer);

    }

    public List<TextAsset> SinglePerceptronSampleDatas;
    public TextAsset NumberData;
    StringDropdown selectModeDropdown()
    {
        StringDropdown dropdown = new StringDropdown("Mode");
        var choices = new List<string> { "None", "SinglePerceptron", "One-Hot DNN", "0~3 Number Predict" };
        var inspector = RuntimeWindow.GetWindow<RuntimeInspector>();
        dropdown.Choices = choices;
        dropdown.OnValueChanged += (newVal) =>
        {
            ModeContainer.Clear();
            if (dropdown.Index == 0)
            {
                inspector.Target = null;
                return;
            }
            if (dropdown.Index == 1)
            {
                var sample = new SinglePerceptronSample(SinglePerceptronSampleDatas);
                ModeContainer.Add(sample);
                inspector.Target = sample.Model;
                return;
            }
            if(dropdown.Index == 2)
            {
                var sample = new OneHotDNNSample();
                ModeContainer.Add(sample);
                inspector.Target = sample.Model;
            }
            if (dropdown.Index == 3)
            {
                var sample = new FourNumberPredictSample(NumberData);
                ModeContainer.Add(sample);
                inspector.Target = null;
            }
        };
        dropdown.Index = 0;
        return dropdown;
    }
}
