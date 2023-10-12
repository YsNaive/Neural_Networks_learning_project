using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class OneHotDNN : TrainableModel<FloatVector, FloatVector>
{
    // input dim, hiddens... , output dim
    public OneHotDNN(params int[] layers)
    {
        if (layers.Length < 2)
            throw new InputLengthNotMatchException($"Must have more then 2 input while create DNN as in/out dimension.");
        Layers = new Dense[layers.Length-1];
        for(int i=1,imax = layers.Length; i < imax; i++)
        {
            Layers[i - 1] = new Dense(layers[i], layers[i - 1], -1);
        }
        Layers[^1].ActivationFunction = new AF_Sigmoid();
    }
    public Dense[] Layers;
    public override ModelResult Eval(FloatVector[] predicts, FloatVector[] answers)
    {
        ModelResult result = new ModelResult();
        int dataSize = predicts.Length;
        int correct = 0;
        float loss = 0;
        for (int i=0; i < dataSize; i++)
        {
            var pre = predicts[i];
            if(pre.IndexOfMaxValue() == answers[i].IndexOfMaxValue())
            {
                correct++;
            }
            else
            {
                float sum = 0;
                int jmax = pre.Length;
                for(int j = 0; j < jmax; j++)
                {
                    sum += (float)Math.Pow(answers[i][j] - pre[j], 2d);
                }
                loss += sum / jmax;
            }
        }
        result.Acc = correct / (float)dataSize;
        result.Loss = loss;
        return result;
    }

    public override FloatVector Predict(FloatVector input)
    {
        FloatVector current = input;
        foreach(var layer in Layers)
        {
            current = layer.Predict(current, layer != Layers[^1]);
        }
        return current;
    }

    public override bool Train(FloatVector input, FloatVector label, float lr)
    {
        if (Predict(input).IndexOfMaxValue() == label.IndexOfMaxValue())
            return false;
        List<FloatVector> predicts = new List<FloatVector>();
        FloatVector current = input;
        foreach(var layer in Layers)
        {
            current = layer.Predict(current, true);
            predicts.Add(current);
        }

        Dictionary<Vector2, float> gdTable = new Dictionary<Vector2, float>();
        bool isOutputLayer = true;
        for(int i= Layers.Length - 1; i >= 0; i--)
        {
            for(int j=0,jmax = Layers[i].NeuralsCount; j < jmax; j++)
            {
                float gd = 0;

                float y = predicts[i][j+1];
                if (isOutputLayer)
                {
                    gd = (label[j] - y) * Layers[i].ActivationFunction.Differential(y);
                }
                else
                {
                    float sum = 0;
                    for (int k = 0, kmax = Layers[i + 1].NeuralsCount; k < kmax; k++)
                    {
                        sum += gdTable[new Vector2(i + 1, k)] * Layers[i + 1].Neurals[k].Weights[j+1];
                    }
                    gd = sum * Layers[i].ActivationFunction.Differential(y);
                }
                gdTable.Add(new Vector2(i,j), gd);
                //ebug.Log("gd " + i + " " + j + " = " + gd);
            }
            isOutputLayer = false;
        }
        for(int i=0,imax = Layers.Length; i < imax; i++)
        {
            FloatVector xs = (i == 0) ? input : predicts[i - 1];

            for(int j=0,jmax = Layers[i].NeuralsCount; j < jmax; j++)
            {
                float dw = gdTable[new Vector2(i, j)];
                dw *= lr;
                FloatVector delta = new FloatVector(Layers[i].Neurals[j].Weights.Length);
                for (int k=0,kmax = Layers[i].Neurals[j].Weights.Length; k < kmax; k++)
                {
                    delta[k] = dw * xs[k];
                }
                Layers[i].Neurals[j].Weights.Add(delta);
            }
        }
        return true;
    }
}
