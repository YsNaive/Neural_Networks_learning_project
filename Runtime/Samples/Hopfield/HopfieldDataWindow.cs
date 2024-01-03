using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static System.Windows.Forms.DataFormats;

public class HopfieldDataWindow : ToolbarWindow
{
    public HopfieldDataBuilder DataBuilder;
    HopfieldDataBuilder testDefault;
    HopfieldDataBuilder basicTestDefault = HopfieldDataBuilder.FromString(BasicTest);
    HopfieldDataBuilder bonusTestDefault = HopfieldDataBuilder.FromString(BonusTest);
    public override void OnCreate()
    {
        base.OnCreate();
        EnableToolbar = false;
        Resizeable = false;
        Dragable = false;
        style.SetIS_Style(ISPadding.Pixel(4));
        style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 2));

        Action repaintInputEditor = null;
        var basicBtn = DocRuntime.NewButton("Basic",DocStyle.Current.HintColor, () =>
        {
            DataBuilder = HopfieldDataBuilder.FromString(BasicTrain);
            testDefault = basicTestDefault;
            repaintInputEditor();
            repaint();
        });
        var bonusBtn = DocRuntime.NewButton("Bonus", DocStyle.Current.HintColor, () =>
        {
            DataBuilder = HopfieldDataBuilder.FromString(BonusTrain);
            testDefault = bonusTestDefault;
            repaintInputEditor();
            repaint();
        });
        var customBtn = DocRuntime.NewButton("Custom", () =>
        {
            SelectSizeWindow.Open(size =>
            {
                var format = new StringBuilder();
                for(int y = 0; y < size.y; y++)
                {
                    for(int x = 0; x < size.x; x++)
                    {
                        format.Append(' ');
                    }
                    format.Append('\n');
                }
                DataBuilder = HopfieldDataBuilder.FromString(format.ToString());
                testDefault = null;
                repaintInputEditor();
                repaint();
            });
        });
        repaintInputEditor = () =>
        {
            HopfieldUI.InputEditor.Container.Clear();
            HopfieldUI.InputData = new HopfieldDataBuilder.Data(DataBuilder.Size);
            HopfieldUI.InputEditor.Open(HopfieldUI.InputData);
            if(testDefault != null)
            {
                var text = DocRuntime.NewTextElement("Default Test Data:");
                HopfieldUI.InputEditor.Container.Add(text);
                text.style.marginLeft = 10;
                text.style.marginTop = 10;
                var container = new VisualElement();
                container.style.marginLeft = 10;
                container.style.flexDirection = FlexDirection.Row;
                container.style.flexWrap = Wrap.Wrap;
                foreach(var data in testDefault.Datas)
                {
                    var mData = data;
                    var size = DocStyle.Current.LineHeight.Value - 4;
                    var btn = new VisualElement();
                    btn.style.SetIS_Style(ISPadding.Pixel(2));
                    btn.Add(data.View);
                    data.ResizeViewLayout(size,0);
                    container.Add(btn);

                    btn.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        for(int x = 0; x < mData.Size.x; x++)
                        {
                            for(int y = 0; y < mData.Size.y; y++)
                            {
                                HopfieldUI.InputData[x, y] = mData[x, y];
                            }
                        }
                    });
                }
                HopfieldUI.InputEditor.Container.Add(container);
            }
        };
        Container.style.marginTop = 5;
        Insert(IndexOf(Container), DocRuntime.NewTextElement("Train Data :"));
        Insert(IndexOf(Container), DocRuntime.NewHorizontalBar(1f, customBtn, basicBtn, bonusBtn));

        DataBuilder = HopfieldDataBuilder.FromString(" ");
        schedule.Execute(() =>
        {
            repaint();
            repaintInputEditor();
        }).ExecuteLater(50);
    }

    void repaint()
    {
        Container.Clear();
        if (DataBuilder == null) return;
        int i = 0;
        if (DataBuilder.Datas.Count == 0)
            DataBuilder.AddEmpty();
        bool isFirst = true;
        foreach(var data in DataBuilder.Datas)
        {
            var mData = data;
            var hor = DocRuntime.NewEmptyHorizontal();
            var item = DocRuntime.NewButton($"data {++i}", () =>
            {
                var edit = GetWindow<HopfieldDataEditorWindow>();
                edit.Open(mData);
            });
            item.Add(data.View);
            item.style.paddingLeft = 10;
            item.style.flexGrow = 1;
            data.EditView.style.height = DocStyle.Current.LineHeight;
            data.ResizeViewLayout(DocStyle.Current.LineHeight.Value,0);
            var removeBtn = DocRuntime.NewButton("x", DocStyle.Current.DangerColor, () =>
            {
                DataBuilder.Datas.Remove(data);
                Container.Remove(hor);
            });
            if (isFirst)
            {
                isFirst = false;
                removeBtn.SetEnabled(false);
            }
            item.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                removeBtn.style.height = evt.newRect.height;
            });
            removeBtn.style.width = DocStyle.Current.LineHeight;
            removeBtn.style.marginLeft = 7;

            hor.Add(item);
            hor.Add(removeBtn);
            Container.Add(hor);
        }
        var addBtn = DocRuntime.NewButton("Add new", DocStyle.Current.SuccessColor, () =>
        {
            DataBuilder.AddEmpty();
            repaint();
        });
        Container.Add(addBtn);
    }

    public static readonly string BasicTrain = "   111   \n  11111  \n 111 111 \n111   111\n11     11\n11     11\n11     11\n11     11\n111111111\n11     11\n11     11\n11     11\n\n   11111 \n  1111111\n111      \n111      \n111      \n111      \n111      \n111      \n111      \n111      \n  1111111\n   11111 \n\n111      \n111      \n111      \n111      \n111      \n111      \n111      \n111      \n111      \n111      \n111111111\n111111111";
    public static readonly string BasicTest  = "   111   \n  1   1  \n 1 1 111 \n1 1   1 1\n11     11\n 1     1 \n11     1 \n 1     11\n11 11111 \n11     11\n1       1\n11      1\n\n   11111 \n  11 1   \n111      \n1 11111  \n1 1   1  \n1 1      \n1 1   1  \n1 1      \n1        \n111      \n  111 1  \n   11  1 \n\n111      \n1 1      \n1 1      \n111   111\n         \n111   111\n         \n      111\n111      \n1 11 11 1\n1 1 1 11 \n11 11 11 ";
    public static readonly string BonusTrain = "1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n\n11  11  11\n11  11  11\n  11  11  \n  11  11  \n11  11  11\n11  11  11\n  11  11  \n  11  11  \n11  11  11\n11  11  11\n\n11111     \n11111     \n11111     \n11111     \n11111     \n     11111\n     11111\n     11111\n     11111\n     11111\n\n1  1  1  1\n 1  1  1  \n  1  1  1 \n1  1  1  1\n 1  1  1  \n  1  1  1 \n1  1  1  1\n 1  1  1  \n  1  1  1 \n1  1  1  1\n\n1111111111\n1        1\n1 111111 1\n1 1    1 1\n1 1 11 1 1\n1 1 11 1 1\n1 1    1 1\n1 111111 1\n1        1\n1111111111\n\n          \n          \n          \n          \n          \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n\n111 1    1\n 1  111 11\n  1 1 11 1\n 111   1  \n11  1  111\n 1 111   1\n1 11  1  1\n   1 111  \n11 111  1 \n 1  1  111\n\n11  11  11\n11  11  11\n  11  11  \n  11  11  \n11  11  11\n11  11  11\n  11  11  \n  11  11  \n11  11  11\n11  11  11\n\n11111     \n1   1 111 \n1   1 111 \n1   1 111 \n11111     \n     11111\n 111 1   1\n 111 1   1\n 111 1   1\n     11111\n\n11111     \n11111     \n11111     \n11111     \n11111     \n     11111\n     11111\n     11111\n     11111\n     11111\n\n1  1111  1\n11  1111  \n111  1111 \n1111  1111\n 1111  111\n  1111  11\n1  1111  1\n11  1111  \n111  1111 \n1111  1111\n\n1  1  1  1\n 1  1  1  \n  1  1  1 \n1  1  1  1\n 1  1  1  \n  1  1  1 \n1  1  1  1\n 1  1  1  \n  1  1  1 \n1  1  1  1\n\n1111111111\n1        1\n1        1\n1        1\n1   11   1\n1   11   1\n1        1\n1        1\n1        1\n1111111111\n\n1111111111\n1        1\n1 111111 1\n1 1    1 1\n1 1 11 1 1\n1 1 11 1 1\n1 1    1 1\n1 111111 1\n1        1\n1111111111";
    public static readonly string BonusTest  = "1 1 1 1 1 \n   1 1   1\n1 1 1 1 1 \n 1 1 1   1\n  1 1 1 1 \n   1 1   1\n1 1 1 1 1 \n       1 1\n  1 1 1 1 \n   1 1 1 1\n\n11  11  11\n          \n  11  11  \n  11  11  \n11  11  11\n11  11  11\n  11  11  \n  11  11  \n          \n11  11  11\n\n11111     \n1  11     \n11111     \n          \n11111     \n     11 11\n     11111\n       111\n     11  1\n     1 111\n\n1  1  1  1\n          \n  1  1  1 \n1        1\n 1  1     \n  1     1 \n1     1  1\n 1     1  \n  1     1 \n1  1     1\n\n1 111   1 \n1        1\n1 111 11 1\n1      1 1\n1 1 11 1 1\n  1 11 1 1\n1 1      1\n1 11  11 1\n1        1\n1 11 1 11 \n\n          \n          \n          \n          \n          \n 1       1\n1       1 \n 1   1 1 1\n1 1   1 1 \n 1 1 1 1 1\n\n1 1 1 1 1 \n   1   1 1\n1 1   1 1 \n 1     1 1\n1       1 \n 1     1 1\n1  1     1\n 1 1 1 1 1\n1 1 1 1 1 \n 1 1 1 1 1\n\n11  1    1\n 1  1   11\n  1 1 11 1\n 1 1   1  \n1   1  111\n 1 1 1   1\n1 11  1  1\n   1 1 1  \n1  1 1  1 \n 1  1  1 1\n\n    11  11\n    11  11\n   1  11  \n   1  11  \n11  11   1\n11  11   1\n   1  11  \n   1  11  \n11   1   1\n11  11   1\n\n11  1     \n    1 1 1 \n    1 1 1 \n1   1 1 1 \n11 11     \n     11 11\n 1 1 1    \n 1 1 1    \n 1 1 1   1\n     11  1\n\n11 11     \n1   1     \n1   1     \n11 11     \n11111     \n     11111\n     11  1\n     1   1\n     1  11\n     11111\n\n1  1111  1\n11  11 1  \n1 1   1 1 \n1 11  11 1\n 1111  1 1\n  1 11  11\n1  1 11  1\n11  1  1  \n1 1  1  1 \n1 11  1  1\n\n1  1  1  1\n       1  \n     1  1 \n1        1\n 1  1  1  \n  1  1  1 \n1     1  1\n 1     1  \n  1     1 \n1  1  1  1\n\n1111111111\n1        1\n1        1\n1        1\n1        1\n1        1\n1        1\n1        1\n1        1\n1111111111\n\n111 111111\n1        1\n1 11 111 1\n1 1    1 1\n1      1 1\n1        1\n1 1    1 1\n1 11 111 1\n1        1\n11 111  11";
}
