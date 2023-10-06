using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActivationFunction
{
    public static float Sigmoid(float x)
    {
        return 1f / (1 + Mathf.Exp(-x));
    }
    public static float Sgn(float x)
    {
        return (x > 0) ? 1f : -1f;
    }
}
