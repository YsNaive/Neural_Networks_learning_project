using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActivationFunction
{
    public static float Sigmoid(float x)
    {
        return (float)(1d / (1d + Math.Exp(-x)));
    }
    public static float UnSigmoid(float x)
    {
        return (float)Math.Log(x / (1d - x));
    }
    public static float Sgn(float x)
    {
        return (x > 0) ? 1f : -1f;
    }
}
