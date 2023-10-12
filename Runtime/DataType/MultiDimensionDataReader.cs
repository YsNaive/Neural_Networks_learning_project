using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MultiDimensionDataReader
{
    /// <summary>
    /// input format : {d1, d2 ,d3 ... dn, group}
    /// </summary>
    public MultiDimensionDataReader(string data, float val_split, bool add_bias = true)
    {
        this.add_bias = add_bias;
        try
        {
            var datas = data.Split(new char[] { '\n' , '\r'});
            var dataSize = datas.Length;
            var splitDatas = new List<string[]>();
            int yCount = 0;
            foreach(var str in datas)
            {
                if (string.IsNullOrEmpty(str)) continue;
                var splitData = str.Split(" ");
                splitDatas.Add(splitData);
                var label = splitData[^1];
                if (!Label2Index.ContainsKey(label))
                {
                    Label2Index.Add(label, yCount);
                    Index2Label.Add(yCount, label);
                    yCount++;
                }
            }
            m_dimensionX = splitDatas[0].Length;
            if (!add_bias) m_dimensionX--;
            m_dimensionY = yCount;
            System.Random rng = new System.Random((int)DateTime.Now.Ticks);
            foreach(var splitData in splitDatas)
            {
                FloatVector x = new FloatVector(DimensionX);
                int offset = 0;
                if (add_bias)
                {
                    x[0] = -1f;
                    offset = 1;
                }
                for(int i=0,imax = DimensionX - offset; i < imax; i++)
                {
                    var fval = float.Parse(splitData[i]);
                    if(fval < min_x)min_x = fval;
                    if(fval > max_x)max_x = fval;
                    x[i + offset] = fval;
                }
                FloatVector y = new FloatVector(DimensionY).ZeroInit();
                y[Label2Index[splitData[^1]]] = 1;
                Data_x.Add(x);
                Data_y.Add(y);
                if (rng.NextDouble() < val_split)
                {
                    Val_x.Add(x);
                    Val_y.Add(y);
                }
                else
                {
                    Train_x.Add(x);
                    Train_y.Add(y);
                }
            }
            IsSuccess = true;
            width_x = max_x - min_x;
        }
        catch
        {
        }
    }
    public bool IsSuccess = false;
    private int m_dimensionX, m_dimensionY;
    public int DimensionX => m_dimensionX;
    public int DimensionY => m_dimensionY;
    public int DataCount => Data_x.Count;
    public float Min_x => min_x;
    public float Max_x => max_x;
    private float min_x = float.MaxValue, max_x = float.MinValue;
    private float width_x;
    private bool add_bias;
    public Dictionary<string, int> Label2Index = new Dictionary<string, int>();
    public Dictionary<int, string> Index2Label = new Dictionary<int, string>();
    public List<FloatVector> Data_x = new List<FloatVector>();
    public List<FloatVector> Data_y = new List<FloatVector>();
    public List<FloatVector> Train_x = new List<FloatVector>();
    public List<FloatVector> Train_y = new List<FloatVector>();
    public List<FloatVector> Val_x = new List<FloatVector>();
    public List<FloatVector> Val_y = new List<FloatVector>();

    public void MinMaxNormalize()
    {
        for(int i=0,imax = DataCount; i < imax; i++)
        {
            for (int j = add_bias?1:0, jmax = Data_x[i].Length; j < jmax; j++) 
                Data_x[i][j] = (Data_x[i][j] - min_x) / width_x;
        }
    }
    public float MinMaxNormalize(float x)
    {
        return (x - min_x) / width_x;
    }
    public float InverseMinMaxNormalize(float normalized_x)
    {
        return normalized_x * width_x + min_x;
    }

    public string Show()
    {
        StringBuilder sb = new StringBuilder();
        for(int i=0,imax = DataCount; i < imax; i++)
        {
            sb.Append(Data_x[i]).Append(" , ").Append(Data_y[i]).Append('\n');
        }
        return sb.ToString();
    }
}
