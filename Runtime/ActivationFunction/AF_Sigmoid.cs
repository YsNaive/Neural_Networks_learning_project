using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AF_Sigmoid : ActivationFunction
{
    public override float Activation(float x)
    {
        return (float)(1d / (1d + Math.Exp(-x)));
    }

    public override float Differential(float activated_x)
    {
        return (activated_x * (1f - activated_x));
    }
}
