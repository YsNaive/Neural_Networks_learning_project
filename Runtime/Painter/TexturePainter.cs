using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter
{
    protected Vector2Int size;
    protected Texture2D texture;
    public Texture2D Texture { get { texture.Apply(); return texture; } }
    public Vector2 Min = new Vector2(-1f,-1f);
    public Vector2 Max = new Vector2(1f,1f);
    public TexturePainter(int width, int height)
    {
        size.x = width;
        size.y = height;
        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
    }

    public void Clear(Color color)
    {
        for(int w = 0; w < size.x; w++)
        {
            for(int h = 0; h < size.y; h++)
            {
                texture.SetPixel(w, h, color);
            }
        }
    }

    public bool DrawPoint(Vector2 pos,Color color)
    {
        if (pos.x < Min.x) return false;
        if (pos.x > Max.x) return false;
        if (pos.y < Min.y) return false;
        if (pos.y > Max.y) return false;
        var posInt = Pos2TexturePos(pos);
        texture.SetPixel(posInt.x, posInt.y, color);
        return true;
    }

    public void DrawLine(Vector2 begin, Vector2 end, Color color)
    {
        Vector2 step = end - begin;
        int stepCount = (int)System.Math.Sqrt(size.sqrMagnitude);
        step *= 1f / stepCount;
        for(int i = -1; i < stepCount; i++)
        {
            if (!DrawPoint(begin, color))
            {
                break;
            }
            begin += step;
        }
    }

    public Vector2Int Pos2TexturePos(Vector2 pos)
    {
        Vector2Int result = new Vector2Int();
        result.x = (int)((pos.x - Min.x) / (Max.x - Min.x) * size.x);
        result.y = (int)((pos.y - Min.y) / (Max.y - Min.y) * size.y);
        return result;
    }
}
