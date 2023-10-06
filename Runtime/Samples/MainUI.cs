using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUI : MonoBehaviour
{
    UIDocument UID;
    public VisualElement Root;
    public ScrollView ModeContainer;
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

        singlePerceptronSample = new SinglePerceptronSample(SinglePerceptronSampleDatas);

        Root.Add(selectModeDropdown());
        Root.Add(ModeContainer);

    }

    SinglePerceptronSample singlePerceptronSample;
    public List<TextAsset> SinglePerceptronSampleDatas;
    StringDropdown selectModeDropdown()
    {
        StringDropdown dropdown = new StringDropdown("Mode");
        var choices = new List<string> { "None", "SinglePerceptron" };
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
                ModeContainer.Add(singlePerceptronSample);
                inspector.Target = singlePerceptronSample.Model;
                return;
            }
        };
        dropdown.Index = 0;
        return dropdown;
    }
}
