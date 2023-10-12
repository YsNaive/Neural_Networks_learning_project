using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelHistoryPainter : TexturePainter
{
    public ModelHistoryPainter(int width, int height) : base(width, height)
    {
    }

    public Color AccColor = Color.green;
    public Color LossColor = Color.red;

    public void DrawHistory(List<ModelResult> history)
    {
        Clear(Color.clear);
        float lossMin = 0, lossMax = history[0].Loss;
        foreach (ModelResult result in history)
        {
            if (result.Loss < lossMin) lossMin = result.Loss;
            if (result.Loss > lossMax) lossMax = result.Loss;
        }
        Min.x = 0;
        int hisSize = history.Count;
        Max.x = Math.Min(size.x, hisSize);

        Min.y = -0.05f; Max.y = 1.05f;
        int ibeg = (hisSize > size.x) ? (hisSize - size.x) : 1;
        Vector2 lastPos = new Vector2(0f, history[ibeg-1].Acc);
        for (int i= ibeg,j=0; i < hisSize; i++,j++)
        {
            Vector2 pos = new Vector2(j, history[i].Acc);
            DrawLine(lastPos, pos, AccColor);
            lastPos = pos;
        }
        var space = (lossMax - lossMin) / 20f;
        Min.y = lossMin-space; Max.y = lossMax+space;
        lastPos = new Vector2(0f, history[ibeg-1].Loss);
        for (int i = ibeg,j=0; i < hisSize; i++,j++)
        {
            Vector2 pos = new Vector2(j, history[i].Loss);
            DrawLine(lastPos, pos, LossColor);
            lastPos = pos;
        }
    }
}
