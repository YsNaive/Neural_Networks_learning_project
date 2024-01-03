using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HopfieldUI : MonoBehaviour
{
    public UIDocument UID;
    VisualElement root;
    public static HopfieldDataEditorWindow InputEditor;
    public static HopfieldDataBuilder.Data InputData;
    void Start()
    {
        root = UID.rootVisualElement;
        root.style.backgroundColor = DocStyle.Current.BackgroundColor;
        root.schedule.Execute(() => { GC.Collect(); }).Every(60);
        var dataEdit = RuntimeWindow.GetWindow<HopfieldDataWindow>();
        dataEdit.LayoutPercent = new Rect(1.5f, 2, 30, 96);
        var dataEditor = RuntimeWindow.GetWindow<HopfieldDataEditorWindow>();
        dataEditor.LayoutPercent = new Rect(30f, 10, 30, 70);
        InputEditor = RuntimeWindow.CreateWindow<HopfieldDataEditorWindow>();
        InputEditor.LayoutPercent = new Rect(32.8f, 2, 35, 96);
        InputEditor.EnableToolbar = false;
        InputEditor.Insert(InputEditor.IndexOf(InputEditor.Container), DocRuntime.NewTextElement("Input Data"));
        var testWindow = RuntimeWindow.GetWindow<HopfieldTestWindow>();
        testWindow.LayoutPercent = new Rect(69, 2, 30, 96);

        IVisualElementScheduledItem scheduledItem = null;
        scheduledItem = dataEditor.schedule.Execute(() => { dataEditor.Close(); scheduledItem.Pause(); });
        scheduledItem.ExecuteLater(50);
    }
}
