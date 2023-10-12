using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class SinglePerceptron : TrainableModel<FloatVector, float>
{
    public SinglePerceptron(int inputCount)
    {
        Neural = new Neural(inputCount);
    }
    public Neural Neural;
    Func<float, float> ActivationFunc = (x) => (x > 0) ? 1 : -1;

    public override float Predict(FloatVector input)
    {
        return ActivationFunc(Neural.Predict(input));
    }

    public override bool Train(FloatVector input, float label, float lr)
    {
        var pre = Predict(input);
        if (ActivationFunc(pre) != label)
        {
            if (pre < 0)
            {
                int jmax = Neural.Weights.Length;
                for (int j = 0; j < jmax; j++)
                {
                    Neural.Weights[j] += input[j] * lr;
                }
            }
            else
            {
                int jmax = Neural.Weights.Length;
                for (int j = 0; j < jmax; j++)
                {
                    Neural.Weights[j] -= input[j] * lr;
                }
            }
            return true;
        }
        return false;
    }

    public override ModelResult Eval(float[] predicts, float[] labels)
    {
        int size = predicts.Length;
        int acc = 0;
        for (int i = 0; i < size; i++)
        {
            if (predicts[i] == labels[i])
                acc++;
        }
        ModelResult modelResult = new ModelResult();
        modelResult.Acc = (float)acc / size;
        return modelResult;
    }


    //// return acc rate
    //public float Eval(List<float[]> inputs, List<float> expectOutputs) {
    //    int size = inputs.Count;
    //    int acc = 0;
    //    for(int i = 0; i < size; i++)
    //    {
    //        if (Predict(inputs[i]) == expectOutputs[i])
    //            acc++;
    //    }
    //    return (float)acc / size;
    //}

    //public void Train(List<float[]> inputs, List<float> expectOutputs)
    //{
    //    int wrongCount = 0;
    //    int size = inputs.Count;
    //    for (int i=0; i < size; i++)
    //    {
    //        var pre = Predict(inputs[i]);
    //        //Debug.Log(pre + " vs "+ expectOutputs[i]);
    //        if (ActivationFunc(pre) != expectOutputs[i])
    //        {
    //            wrongCount++;
    //            if(pre < 0)
    //            {
    //                int jmax = Neural.Weights.Length;
    //                for (int j = 0; j < jmax; j++)
    //                {
    //                    Neural.Weights[j] += inputs[i][j] * LearningRate;
    //                }
    //            }
    //            else
    //            {
    //                int jmax = Neural.Weights.Length;
    //                for (int j = 0; j < jmax; j++)
    //                {
    //                    Neural.Weights[j] -= inputs[i][j] * LearningRate;
    //                }
    //            }
    //        }
    //    }
    //}
}
