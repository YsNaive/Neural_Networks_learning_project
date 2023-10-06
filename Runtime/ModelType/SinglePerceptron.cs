using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class SinglePerceptron
{
    public SinglePerceptron(int inputCount)
    {
        Neural = new Neural(inputCount);
    }
    public Neural Neural;
    public float LearningRate = 0.01f;
    Func<float, float> ActivationFunc = ActivationFunction.Sgn;
    public float Predict(float[] input)
    {
        return ActivationFunc(Neural.Predict(input));
    }

    // return acc rate
    public float Eval(List<float[]> inputs, List<float> expectOutputs) {
        int size = inputs.Count;
        int acc = 0;
        for(int i = 0; i < size; i++)
        {
            if (Predict(inputs[i]) == expectOutputs[i])
                acc++;
        }
        return (float)acc / size;
    }

    public void Train(List<float[]> inputs, List<float> expectOutputs)
    {
        int wrongCount = 0;
        int size = inputs.Count;
        for (int i=0; i < size; i++)
        {
            var pre = Predict(inputs[i]);
            //Debug.Log(pre + " vs "+ expectOutputs[i]);
            if (ActivationFunc(pre) != expectOutputs[i])
            {
                wrongCount++;
                if(pre < 0)
                {
                    int jmax = Neural.Weights.Length;
                    for (int j = 0; j < jmax; j++)
                    {
                        Neural.Weights[j] += inputs[i][j] * LearningRate;
                    }
                }
                else
                {
                    int jmax = Neural.Weights.Length;
                    for (int j = 0; j < jmax; j++)
                    {
                        Neural.Weights[j] -= inputs[i][j] * LearningRate;
                    }
                }
            }
        }
    }
}
