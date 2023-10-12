using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AF_ReLU : ActivationFunction
{
    public override float Activation(float x)
    {
        return ((x > 0) ? x : 0);
    }

    public override float Differential(float activated_x)
    {
        return ((activated_x > 0) ? 1 : 0);
    }

    public override float InverseActivation(float activated_x)
    {
        return activated_x;
    }
}
