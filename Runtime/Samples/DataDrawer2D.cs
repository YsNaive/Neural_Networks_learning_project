using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public static class DataDrawer2D
{
    public static Texture2D Buffer
    {
        get
        {
            buffer.Apply();
            return buffer;
        }
    }
    static Texture2D buffer;
    static int w, h;
    public static Color Color;
    public static float XMin => xMin;
    public static float XMax => xMax;
    public static float YMin => yMin;
    public static float YMax => yMax;
    private static float xMin = -10, xMax = 10, yMin = -10, yMax = 10, xScale, yScale;
    public static void SetXRange(float min, float max) { xMin = min; xMax = max; calScale(); }
    public static void SetYRange(float min, float max) { yMin = min; yMax = max; calScale(); }
    public static void NewTexture(int w, int h) { NewTexture(w, h, Color.white); }
    public static void NewTexture(int w, int h, Color color)
    {
        buffer = new Texture2D(w, h);
        DataDrawer2D.w = w;
        DataDrawer2D.h = h; 
        calScale();
        Clear(color);
    }
    public static void Clear() { Clear(Color.white); }
    public static void Clear(Color color)
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                buffer.SetPixel(i, j, color);
            }
        }
    }
    public static void DrawPoints(params Vector2[] points)
    {
        foreach(var p in points)
        {
            if (p.x < xMin) continue;
            if (p.x > xMax) continue;
            if (p.y < yMin) continue;
            if (p.y > yMax) continue;
            buffer.SetPixel((int)((p.x - xMin) * xScale), (int)((p.y - yMin) * yScale), Color);
        }
    }
    public static void DrawLine(Vector2 p1, Vector2 p2)
    {
        float xRange = Mathf.Abs(p1.x - p2.x);
        float yRange = Mathf.Abs(p1.y - p2.y);
        int len;
        if (xRange > yRange)
            len = (int)(xRange * xScale) + 1;
        else
            len = (int)(yRange * yScale) + 1;
        float xStep = (p2.x - p1.x) / len;
        float yStep = (p2.y - p1.y) / len;
        for (int i = 0; i < len; i++)
        {
            DrawPoints(p1);
            p1.x += xStep;
            p1.y += yStep;
        }
    }

    static void calScale()
    {
        xScale = (1f / (xMax - xMin)) * w;
        yScale = (1f / (yMax - yMin)) * w;
    }
}
