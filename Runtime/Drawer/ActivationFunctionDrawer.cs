using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(ActivationFunction),true)]
public class ActivationFunctionDrawer : RuntimeDrawer<ActivationFunction>
{
    static ActivationFunctionDrawer()
    {
        typeChoices = DocRuntime.FindAllTypesWhere((type) => { return (type.IsSubclassOf(typeof(ActivationFunction)) && !type.IsAbstract); });
        foreach (var item in typeChoices)
            choices.Add(item.Name);
    }
    static List<Type> typeChoices = new List<Type>();
    static List<string> choices = new List<string>();
    public override bool Repaintable => false;
    public override void InitGUI(string label, VisualElement root)
    {
        var dropdown = new StringDropdown(label);
        dropdown.Choices = choices;
        OnReferenceChanged += () => { dropdown.Value = value.GetType().Name; };
        dropdown.OnValueChanged += (val) =>
        {
            SetValueWithoutNotify((ActivationFunction)Activator.CreateInstance(typeChoices[dropdown.Index]));
        };
        root.Add(dropdown);
    }

    public override void Repaint()
    {
    }
}
