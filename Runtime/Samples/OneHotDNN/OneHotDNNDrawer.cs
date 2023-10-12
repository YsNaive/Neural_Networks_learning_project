using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
[CustomRuntimeDrawer(typeof(OneHotDNN))]
public class OneHotDNNDrawer : RuntimeDrawer<OneHotDNN>
{
    public override bool Repaintable => true;
    public DenseDrawer[] denseDrawers;
    public override void InitGUI(string label, VisualElement root)
    {
        Add(DocRuntime.NewTextElement("DNN model"));
        OnReferenceChanged += () =>
        {
            denseDrawers = new DenseDrawer[value.Layers.Length];
            for(int i=0,imax = denseDrawers.Length; i < imax; i++)
            {
                var drawer = (DenseDrawer)CreateDrawer($"Layer {i + 1} (Dense)", value.Layers[i]);
                denseDrawers[i] = drawer;
                root.Add(drawer);
            }
        };
    }

    public override void Repaint()
    {
        if (denseDrawers == null) return;
        foreach(var drawer in denseDrawers)
            drawer.Repaint();
    }
}
