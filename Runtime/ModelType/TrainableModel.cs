using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class TrainableModel<Tin,Tout>
{
    public class Trainer : ModelTrainer<Tin, Tout>
    {
        public Trainer(TrainableModel<Tin, Tout> model) : base(model)
        {
        }
    }
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
    public abstract bool Train(Tin input, Tout label, float lr);
    public virtual ModelResult Train(List<Tin> inputs, List<Tout> labels, float lr) { return Train(inputs.ToArray(), labels.ToArray(), lr); }
    public virtual ModelResult Train(Tin[] inputs, Tout[] labels, float lr)
    {
        int len = inputs.Length;
        int effectCount = 0;
        for (int i = 0; i < len; i++)
        {
            if(Train(inputs[i], labels[i], lr))
            {
                effectCount++;
            }
        }
        return Eval(Predict(inputs),labels);
    }
    public abstract ModelResult Eval(Tout[] predicts, Tout[] answers);
}
