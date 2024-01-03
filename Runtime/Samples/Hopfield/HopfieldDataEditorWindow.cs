using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class HopfieldDataEditorWindow : ToolbarWindow
{
    public override void OnCreate()
    {
        Name = "Data Editor";
        base.OnCreate();
        Resizeable = false;
        Dragable = false;
        EnableRightClickMenu = false;
        style.SetIS_Style(ISPadding.Pixel(4));
        style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2));
    }
    public void Open(HopfieldDataBuilder.Data target)
    {
        Container.Clear();
        var clearBtn = DocRuntime.NewButton("Clear", () =>
        {
            for(int y = 0; y < target.Size.y; y++)
            {
                for(int x=0;x< target.Size.x; x++)
                {
                    target[x, y] = false;
                }
            }
        });
        clearBtn.style.marginLeft = 10;
        clearBtn.style.width = 80;
        var fillBtn = DocRuntime.NewButton("Fill", () =>
        {
            for (int y = 0; y < target.Size.y; y++)
            {
                for (int x = 0; x < target.Size.x; x++)
                {
                    target[x, y] = true;
                }
            }
        });
        fillBtn.style.marginLeft = 10;
        fillBtn.style.width = 80;
        var randBtn = DocRuntime.NewButton("Random", () =>
        {
            for (int y = 0; y < target.Size.y; y++)
            {
                for (int x = 0; x < target.Size.x; x++)
                {
                    target[x, y] = Random.Range(0,2) == 0;
                }
            }
        });
        randBtn.style.marginLeft = 10;
        randBtn.style.width = 100;
        var hor = DocRuntime.NewEmptyHorizontal();
        hor.Add(clearBtn);
        hor.Add(fillBtn);
        hor.Add(randBtn);
        Container.Add(hor);
        Container.Add(target.EditView);
        target.ResizeEditLayout(Container.worldBound.width, Container.worldBound.width * 0.01f);
    }
}
