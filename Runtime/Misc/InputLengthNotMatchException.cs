using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLengthNotMatchException : Exception
{
    public InputLengthNotMatchException()
    {
    }

    public InputLengthNotMatchException(string message)
        : base(message)
    {
    }
    public InputLengthNotMatchException(int inLen, int requireLen)
        : base($"input size {inLen} not match requir {requireLen}.")
    {
    }
}
