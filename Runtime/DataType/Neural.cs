using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Neural
{
    public Neural()
    {
        weights = new float[1];
        Bias = -1;
    }
    public Neural(int inputLength, float bias = -1)
    {
        weights = new float[inputLength+1];
        Bias = bias;
    }

    public float Bias
    {
        get => weights[0];
        set => weights[0] = value;
    }
    public float[] Weights => weights;
    [SerializeField] private float[] weights;

    public float Predict(float[] input)
    {
        int len = input.Length;
        if (len != Weights.Length)
            throw new InputLengthNotMatchException(len, Weights.Length);

        float result = 0;
        for(int i=0; i < len; i++)
        {
            result += Weights[i] * input[i];
        }
        return result;
    }
}
