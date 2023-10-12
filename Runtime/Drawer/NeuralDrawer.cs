using NaiveAPI.Runtime_Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(Neural))]
public class NeuralDrawer : FoldoutDrawer<Neural>
{
    public override bool Repaintable => true;
    FloatDrawer[] weightDrawers;
    public override void InitGUI(string label, VisualElement root)
    {
        base.InitGUI(label, root);
        OnReferenceChanged += () =>
        {
            root.Clear();
            weightDrawers = new FloatDrawer[value.Weights.Length];
            for (int i = 0, imax = weightDrawers.Length; i < imax; i++)
            {
                var drawer = (FloatDrawer)CreateDrawer((i == 0) ? "bias" : $"w{i}", typeof(float));
                weightDrawers[i] = drawer;
                root.Add(drawer);
                int curi = i;
                drawer.OnValueChanged += () =>
                {
                    value.Weights[curi] = drawer.value;
                };
            }
        };
    }
    public override void Repaint()
    {
        if (weightDrawers == null) return;
        for(int i = 0, imax = weightDrawers.Length; i < imax; i++)
            weightDrawers[i].value = value.Weights[i];
    }
}
