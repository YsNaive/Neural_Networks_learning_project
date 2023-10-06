using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomRuntimeDrawer(typeof(SinglePerceptron),false)]
public class SinglePerceptronDrawer : RuntimeDrawer<SinglePerceptron>
{
    public override bool Repaintable => true;
    FloatDrawer[] drawers = new FloatDrawer[4];// bias, x1, x2, lr
    public override void InitGUI(string label, VisualElement root)
    {
        Add(DocRuntime.NewTextElement("Perceptron"));
        drawers[0] = (FloatDrawer)CreateDrawer("Bias", typeof(float));
        drawers[1] = (FloatDrawer)CreateDrawer("w1", typeof(float));
        drawers[2] = (FloatDrawer)CreateDrawer("w2", typeof(float));
        drawers[2].style.marginBottom = 10;
        for (int i = 0; i < 3; i++)
        {
            int curi = i;
            drawers[curi].OnValueChanged += () => { 
                value.Neural.Weights[curi] = drawers[curi].value; 
            };
        }
            
        drawers[3] = (FloatDrawer)CreateDrawer("LR", typeof(float));
        drawers[3].OnValueChanged += () => { value.LearningRate = drawers[3].value; };

        foreach(var drawer in drawers)
            Add(drawer);

        OnReferenceChanged += Repaint;
    }

    public override void Repaint()
    {
        for (int i = 0; i < 3; i++)
            drawers[i].value = value.Neural.Weights[i];

        drawers[3].value = value.LearningRate;
    }
}
