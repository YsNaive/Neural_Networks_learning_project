using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectSizeWindow : ToolbarWindow
{
    public Vector2IntDrawer Drawer;
    public override void OnCreate()
    {
        Name = "Edit Size";
        base.OnCreate();
        DocStyle.Current.BeginLabelWidth(ISLength.Pixel(65));
        Drawer = (Vector2IntDrawer)RuntimeDrawer.CreateDrawer("Size", new Vector2Int());
        DocStyle.Current.EndLabelWidth();
        Insert(IndexOf(Container),Drawer);
        Resizeable = false;
        Dragable = true;
        LayoutPercent = new Rect(10, 20, 40, 25);
        style.SetIS_Style(ISPadding.Pixel(4));
        style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2));
        EnableRightClickMenu = false;
    }
    public static void Open(Action<Vector2Int> callback)
    {
        var window = GetWindow<SelectSizeWindow>();
        window.Container.Clear();
        var okBtn = DocRuntime.NewButton("OK", DocStyle.Current.SuccessColor, () =>
        {
            callback?.Invoke(window.Drawer.value);
            window.Close();
        });
        var cancelBtn = DocRuntime.NewButton("Cancel", DocStyle.Current.DangerColor, () =>
        {
            window.Close();
        });
        window.Container.Add(DocRuntime.NewHorizontalBar(4f, okBtn, cancelBtn));
    }
}
