using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class TrainableModel<Tin,Tout>
    where Tout : new()
{
    public float LearningRate = 0.01f;
    public int Epochs = 50;

    public abstract Tout Predict(Tin input);
    public virtual Tout[] Predict(List<Tin> input) { return Predict(input.ToArray()); }
    public virtual Tout[] Predict(params Tin[] input)
    {
        int len = input.Length;
        Tout[] result = new Tout[len];
        for(int i=0; i < len; i++)
        {
            result[i] = Predict(input[i]);
        }
        return result;
    }
    // return T if this train effect the model
    public abstract bool Train(Tin input, Tout label);
    public virtual ModelResult Train(List<Tin> inputs, List<Tout> labels) { return Train(inputs.ToArray(), labels.ToArray()); }
    public virtual ModelResult Train(Tin[] inputs, Tout[] labels)
    {
        int len = inputs.Length;
        ModelResult result = new ModelResult();
        int effectCount = 0;
        for (int i = 0; i < len; i++)
        {
            if(Train(inputs[i], labels[i]))
            {
                effectCount++;
            }
        }
        result.Acc = effectCount/(float)len;
        return result;
    }
    public abstract ModelResult Eval(Tout[] predicts, Tout[] answers);
}
