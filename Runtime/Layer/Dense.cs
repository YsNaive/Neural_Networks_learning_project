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
    public ActivationFunction ActivationFunction = new AF_ReLU();
    public Neural[] Neurals;
    private int inputDim;
    public int NeuralsCount => Neurals.Length;
    public FloatVector Predict(FloatVector input, bool add_bias = false)
    {
        if(input.Length != inputDim)
            throw new InputLengthNotMatchException(input.Length, inputDim);

        FloatVector result = new FloatVector(Neurals.Length + (add_bias ? 1 : 0));
        int offset = (add_bias? 1 : 0);
        if (add_bias) result[0] = -1f;
        for(int i = 0,imax = NeuralsCount; i < imax; i++)
        {
            result[i + offset] = (ActivationFunction.Activation(Neurals[i].Predict(input)));
        }
        return result;
    }
}
