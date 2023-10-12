using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelResult
{
    public float Loss = 0f;
    public float Acc = 0f;

    public override string ToString()
    {
        return $"Acc {Acc}\tLoss {Loss}";
    }
}
