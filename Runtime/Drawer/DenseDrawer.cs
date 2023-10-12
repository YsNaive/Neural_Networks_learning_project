using NaiveAPI.Runtime_Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(Dense))]
public class DenseDrawer : FoldoutDrawer<Dense>
{
    public override bool Repaintable => true;
    NeuralDrawer[] neuralDrawers;
    public override void InitGUI(string label, VisualElement root)
    {
        base.InitGUI(label, root);
        OnReferenceChanged += () =>
        {
            root.Clear();
            root.Add(CreateDrawer("Activation", value.ActivationFunction));
            FoldoutElement.text = $"Layer (Dense, {value.NeuralsCount})";
            neuralDrawers = new NeuralDrawer[value.NeuralsCount];
            for(int i=0,imax = neuralDrawers.Length; i < imax; i++)
            {
                NeuralDrawer drawer = (NeuralDrawer)CreateDrawer($"Neural {i}", value.Neurals[i]);
                neuralDrawers[i] = drawer;
                root.Add(drawer);
            }
        };
    }
    public override void Repaint()
    {
        if (value == null) return;
        foreach(var drawer in neuralDrawers)
            drawer.Repaint();
    }
}
