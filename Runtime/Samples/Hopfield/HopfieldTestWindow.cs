using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class HopfieldTestWindow : ToolbarWindow
{
    public override void OnCreate()
    {
        base.OnCreate();
        Resizeable = false;
        Dragable = false;
        EnableRightClickMenu = false;
        EnableToolbar = false;
        style.SetIS_Style(ISPadding.Pixel(4));
        style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2));

        var recallBtn = DocRuntime.NewButton("Recall", DocStyle.Current.HintColor, () =>
        {
            Container.Clear();
            var dataSet = GetWindow<HopfieldDataWindow>().DataBuilder;
            Hopfield model = new Hopfield(dataSet);
            var result = model.Recall(HopfieldUI.InputData.Datas);
            var stepsView = DocRuntime.NewEmptyHorizontal();
            stepsView.style.flexWrap = Wrap.Wrap;
            HopfieldDataBuilder.Data stepData = null;
            var width = Container.worldBound.width / 5f;
            var spacing = width * 0.05f;
            width*= 0.9f;
            foreach (var step in result)
            {
                stepData = new HopfieldDataBuilder.Data(step);
                stepData.ResizeViewLayout(width, 0);
                stepData.View.style.marginLeft = spacing;
                stepData.View.style.marginRight = spacing;
                stepData.View.style.marginTop = spacing;
                stepData.View.style.marginBottom = spacing;
                stepsView.Add(stepData.View);
            }
            Container.Add(stepsView);

            HopfieldDataBuilder.Data bestRecall = null;
            float bestAcc = 0;
            foreach(var data in dataSet.Datas)
            {
                var score = model.CompareData(data.Datas, stepData.Datas);
                if(score > bestAcc)
                {
                    bestAcc = score;
                    bestRecall = data;
                }
            }
            var bestAns = new HopfieldDataBuilder.Data(bestRecall.Datas);
            bestRecall = new HopfieldDataBuilder.Data(bestRecall.Datas);
            for(int y = 0; y < bestRecall.Size.y; y++)
            {
                for(int x = 0; x < bestRecall.Size.x; x++)
                {
                    var unit = bestRecall.View[y][x];
                    if (bestRecall[x,y] == stepData[x, y])
                        unit.style.backgroundColor = bestRecall[x, y] ? DocStyle.Current.SuccessTextColor : DocStyle.Current.SuccessColor;
                    else
                        unit.style.backgroundColor = bestRecall[x, y] ? DocStyle.Current.DangerColor : DocStyle.Current.DangerTextColor;
                }
            }
            var before = HopfieldUI.InputData;
            var widthPx = width * 2.2f;
            var spacePx = width * 0.3f / 3f;
            var border = new ISBorder(DocStyle.Current.SubBackgroundColor, spacePx/2f);
            before.ResizeViewLayout(widthPx, 0);
            before.View.style.SetIS_Style(border);
            stepData.ResizeViewLayout(widthPx, 0);
            stepData.View.style.SetIS_Style(border);
            stepData.View.style.ClearMargin();
            stepData.View.style.marginLeft = spacePx;
            bestAns.ResizeViewLayout(widthPx, 0);
            bestAns.View.style.SetIS_Style(border);
            bestRecall.ResizeViewLayout(widthPx, 0);
            bestRecall.View.style.SetIS_Style(border);
            bestRecall.View.style.marginLeft = spacePx;
            var resultHor = DocRuntime.NewEmptyHorizontal();
            var comHor = DocRuntime.NewEmptyHorizontal();
            resultHor.style.marginTop = 7;
            resultHor.Add(before.View);
            resultHor.Add(stepData.View);
            comHor.Add(bestAns.View);
            comHor.Add(bestRecall.View);
            Container.Add(DocRuntime.NewTextElement("Before -> After"));
            Container.Add(resultHor);
            Container.Add(DocRuntime.NewTextElement($"Best Match {(int)(bestAcc*100)}%"));
            Container.Add(comHor);
        });
        Insert(IndexOf(Container), recallBtn);
    }
}
