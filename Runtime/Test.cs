using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
public class Test : MonoBehaviour
{
    public (int,int) foo()
    {
        return (1, 2);
    }

    void Start()
    {
        (int x, int y) = foo();
        
        var result = foo();
        Debug.Log(result.GetType());
    }
}
