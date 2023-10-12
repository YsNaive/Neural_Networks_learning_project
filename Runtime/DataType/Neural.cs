using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Neural
{
    public Neural()
    {
        weights = new FloatVector(1);
        Bias = -1;
    }
    public Neural(int inputLength, float bias = -1)
    {
        weights = new FloatVector(inputLength+1).RandomInit();
        Bias = bias;
    }

    public float Bias
    {
        get => weights[0];
        set => weights[0] = value;
    }
    public FloatVector Weights => weights;
    [SerializeField] private FloatVector weights;

    public float Predict(FloatVector input)
    {
        if (input.Length != Weights.Length)
            throw new InputLengthNotMatchException(input.Length, Weights.Length);
        return FloatVector.Dot(Weights, input);
    }
}
