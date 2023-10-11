using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigmoid : ActivationFunction
{
    public override float Activation(float x)
    {
        return (float)(1d / (1d + Math.Exp(-x)));
    }

    public override float Differential(float activated_x)
    {
        return (activated_x * (1f - activated_x));
    }

    public override float InverseActivation(float activated_x)
    {
        return (float)Math.Log(activated_x / (1d - activated_x));
    }
}
