using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AF_None : ActivationFunction
{
    public override float Activation(float x)
    {
        return x;
    }

    public override float Differential(float activated_x)
    {
        return 1;
    }
}
