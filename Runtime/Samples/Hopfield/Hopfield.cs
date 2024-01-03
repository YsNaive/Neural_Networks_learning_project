using NaiveAPI.Runtime_Window;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Hopfield
{
    float[,] weight;
    int dataX, dataY, dataLen, dataCount;
    HopfieldDataBuilder dataSet;
    public Hopfield(HopfieldDataBuilder dataSet)
    {
        this.dataSet = dataSet;
        dataX = (int)(dataSet.Datas[0].Size.x);
        dataY = (int)(dataSet.Datas[0].Size.y);
        dataLen = dataX * dataY;
        dataCount = dataSet.Datas.Count;
        var inputMatrix = new float[dataCount, dataLen];
        int i = 0;
        foreach (var data in dataSet.Datas)
        {
            for (int y = 0; y < dataY; y++)
            {
                var index = y * dataX;
                for (int x = 0; x < dataX; x++)
                {
                    inputMatrix[i, index++] = data[x, y] ? 1 : -1;
                }
            }
            i++;
        }
        weight = new float[dataLen, dataLen];
        for(int y = 0; y < dataLen; y++)
        {
            for(int x = 0; x < dataLen; x++)
            {
                weight[y, x] = 0;
                if (x == y) continue;
                for (i = 0; i < dataCount; i++)
                {
                    weight[y, x] += inputMatrix[i,x] * inputMatrix[i,y];
                }
            }
        }
    }
    public float CompareData(bool[,] lhs, bool[,] rhs)
    {
        int sameCount = 0;
        for (int y = 0; y < dataY; y++)
        {
            for (int x = 0; x < dataX; x++)
            {
                if (lhs[y,x] == rhs[y,x])
                    sameCount++;
            }
        }
        return sameCount / (float)dataLen;
    }
    public List<bool[,]> Recall(bool[,] input) { return Recall(input, new List<bool[,]>()); }
    public List<bool[,]> Recall(bool[,] input, List<bool[,]> step)
    {
        var inputMatrix = new float[dataLen];
        var result = new float[dataLen];
        for (int y = 0; y < dataY; y++)
        {
            for (int x = 0; x < dataX; x++)
            {
                inputMatrix[y * dataX + x] = input[y, x] ? 1 : -1;
            }
        }

        for(int w = 0; w < dataLen; w++)
        {
            result[w] = 0;
            for(int i = 0; i < dataLen; i++)
            {
                result[w] += weight[w, i] * inputMatrix[i];
            }
        }
        var boolResult = new bool[dataY, dataX];
        for (int y = 0; y < dataY; y++)
        {
            for (int x = 0; x < dataX; x++)
            {
                boolResult[y, x] = result[x + y * dataX] > 0;
            }
        }
        step.Add(boolResult);
        if (CompareData(boolResult, input) == 1 || step.Count >= 13)
            return step;
        else
            return Recall(boolResult, step);
    }
}
