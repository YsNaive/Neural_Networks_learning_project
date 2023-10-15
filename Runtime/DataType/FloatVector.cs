using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class FloatVector
{
    public FloatVector(int length)
    {
        m_values = new float[length];
        m_length = length;
    }
    [SerializeField]private float[] m_values;
    private int m_length;

    public float[] Values => m_values;
    public int Length => m_length;
    public float this[int index]
    {
        get => m_values[index];
        set => m_values[index] = value;
    }
    public void Add(FloatVector other)
    {
        sizeCheck(this, other);
        for (int i = 0, imax = this.Length; i < imax; i++)
        {
            this[i] += other[i];
        }
    }
    public void Sub(FloatVector other)
    {
        sizeCheck(this, other);
        for (int i = 0, imax = this.Length; i < imax; i++)
        {
            this[i] -= other[i];
        }
    }
    public FloatVector ZeroInit(float val = 0f)
    {
        for (int i = 0, imax = this.Length; i < imax; i++)
        {
            this[i] = val;
        }
        return this;
    }
    public FloatVector RandomInit(float min = -1f, float max = 1f)
    {
        System.Random rng = new System.Random();
        float wid = max - min;
        for (int i = 0, imax = this.Length; i < imax; i++)
        {
            this[i] = (float)rng.NextDouble() * (wid) + min;
        }
        return this;
    }

    public int IndexOfMaxValue()
    {
        if (this.Length == 0) return -1;
        int index = 0;
        float max = this[0];
        for (int i = 1, imax = this.Length; i < imax; i++)
        {
            if (this[i] > max)
            {
                max = this[i];
                index = i;
            }
        }
        return index;
    }

    private static void sizeCheck(FloatVector lhs, FloatVector rhs)
    {
        if (lhs.Length != rhs.Length)
            throw new InputLengthNotMatchException($"Can't Dot 2 diffent length vector, {lhs.Length} vs {rhs.Length}");
    }

    public static float Dot(FloatVector lhs, FloatVector rhs)
    {
        sizeCheck(lhs, rhs);
        float result = 0;
        for (int i = 0, imax = lhs.Length; i < imax; i++)
        {
            result += lhs[i] * rhs[i];
        }
        return result;
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{ ");
        foreach (var v in m_values) { sb.Append(v.ToString()).Append(", "); }
        sb.Append("}");
        return sb.ToString();
    }
}
