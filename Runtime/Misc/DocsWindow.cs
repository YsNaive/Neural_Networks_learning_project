using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocsWindow : ToolbarWindow
{
    public override void OnCreate()
    {
        Name = "NN Learning Documentation";
        base.OnCreate();
        Dragable = false;
        Resizeable = false;
        EnableToolbar = true;
        LayoutPercent = new Rect(0, 0, 100, 100);
        //var book = new DocBookVisual(RuntimeWindowManager.Instance.DocsRoot);
        //book.DontPlayAnimation = true;
        //Add(book);
    }
}
