using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivationFunction
{
    public abstract float Activation(float x);
    public abstract float Differential(float activated_x);
}