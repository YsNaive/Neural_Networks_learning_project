using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;

public class MultiDimensionDataReader
{
    /// <summary>
    /// input format : {d1, d2 ,d3 ... dn, group}
    /// </summary>
    public MultiDimensionDataReader(string data, float val_split, bool add_bias = true)
    {
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
                    x[i + offset] = float.Parse(splitData[i]);
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
        }
        catch
        {
            throw new System.Exception("MultiDimensionDataReader Fail, please check the input format.");
        }
    }
    private int m_dimensionX, m_dimensionY;
    public int DimensionX => m_dimensionX;
    public int DimensionY => m_dimensionY;
    public int DataCount => Data_x.Count;
    public Dictionary<string, int> Label2Index = new Dictionary<string, int>();
    public Dictionary<int, string> Index2Label = new Dictionary<int, string>();
    public List<FloatVector> Data_x = new List<FloatVector>();
    public List<FloatVector> Data_y = new List<FloatVector>();
    public List<FloatVector> Train_x = new List<FloatVector>();
    public List<FloatVector> Train_y = new List<FloatVector>();
    public List<FloatVector> Val_x = new List<FloatVector>();
    public List<FloatVector> Val_y = new List<FloatVector>();
}
