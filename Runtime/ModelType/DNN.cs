using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DNN : TrainableModel<List<float>, List<float>>
{
    // input dim, hiddens... , output dim
    public DNN(params int[] layers)
    {
        if (layers.Length < 2)
            throw new InputLengthNotMatchException($"Must have more then 2 input while create DNN as in/out dimension.");
        Layers = new Dense[layers.Length-1];
        for(int i=1,imax = layers.Length; i < imax; i++)
        {
            Layers[i - 1] = new Dense(layers[i], layers[i - 1], -1);
        }

    }

    public Dense[] Layers;
    public override ModelResult Eval(List<float>[] predicts, List<float>[] answers)
    {
        //int sum = 0;
        //for(int i=0,imax = predicts.Length; i < imax; i++)
        //{
        //    if (ActivationFunction.Sgn(predicts[i][0]) != answers[i][0])
        //    {
        //        Debug.Log(predicts[i][0] + " vs " + answers[i][0]);
        //        sum++;
        //    }
        //}
        //Debug.Log(sum + " / " + predicts.Length);
        return null;
    }

    public override List<float> Predict(List<float> input)
    {
        List<float> current = input;
        foreach(var layer in Layers)
        {
            current = layer.Predict(current);
            current.Insert(0, -1);
        }
        current.RemoveAt(0);
        return current;
    }

    public override bool Train(List<float> input, List<float> label)
    {
        if (MaxIndex(Predict(input)) == MaxIndex(label))
            return false;
        List<List<float>> predicts = new List<List<float>>();
        List<float> current = input;
        foreach(var layer in Layers)
        {
            current = layer.Predict(current);
            current.Insert(0, -1f);
            predicts.Add(current);
        }

        //foreach(var pre in predicts)
        //{
        //    string str = "";
        //    foreach (var a in pre)
        //        str += $"{a} ";
        //    Debug.Log(str);
        //}
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
                    gd = (label[j] - y) * y*(1f-y);
                }
                else
                {
                    float sum = 0;
                    for (int k = 0, kmax = Layers[i + 1].NeuralsCount; k < kmax; k++)
                    {
                        sum += gdTable[new Vector2(i + 1, k)] * Layers[i + 1].Neurals[k].Weights[j];
                    }
                    gd = sum * y * (1f - y);
                }
                gdTable.Add(new Vector2(i,j), gd);
                //ebug.Log("gd " + i + " " + j + " = " + gd);
            }
            isOutputLayer = false;
        }
        for(int i=0,imax = Layers.Length; i < imax; i++)
        {
            List<float> xs = (i == 0) ? input : predicts[i - 1];

            for(int j=0,jmax = Layers[i].NeuralsCount; j < jmax; j++)
            {
                float dw = gdTable[new Vector2(i, j)];
                dw *= LearningRate;
                for (int k=0,kmax = Layers[i].Neurals[j].Weights.Length; k < kmax; k++)
                {
                    Layers[i].Neurals[j].Weights[k] += dw * xs[k];
                }
            }
        }
        return true;
    }
    public int MaxIndex(List<float> input)
    {
        int index = 0;
        float max = input[0];
        for(int i=0,imax = input.Count;i< imax;i++)
        {
            if (input[i] > max)
            {
                max = input[i];
                index = i;
            }
        }
        return index;
    }
}
