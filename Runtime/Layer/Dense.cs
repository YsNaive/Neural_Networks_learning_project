using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dense : Layer
{
    public Dense(int neuralCount, int inputDimension, float bias = -1)
    {
        inputDim = inputDimension+1;
        Neurals = new Neural[neuralCount];
        for (int i = 0; i < neuralCount; i++)
            Neurals[i] = new Neural(inputDimension, bias);
    }
    public ActivationFunction ActivationFunction = new Sigmoid();
    public Neural[] Neurals;
    private int inputDim;
    public int NeuralsCount => Neurals.Length;
    public List<float> Predict(List<float> input)
    {
        if(input.Count != inputDim)
            throw new InputLengthNotMatchException(input.Count, inputDim);

        List<float> result = new List<float>();
        for(int i = 0,imax = NeuralsCount; i < imax; i++)
        {
            result.Add(ActivationFunction.Activation(Neurals[i].Predict(input)));
        }
        return result;
    }
}
