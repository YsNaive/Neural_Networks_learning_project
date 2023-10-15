using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class FourNumberPredictSample : VisualElement
{
    OneHotDNN model = new OneHotDNN(25, 7, 4);
    TextElement showResult;
    public FourNumberPredictSample(TextAsset data) {
        this.style.SetIS_Style(ISMargin.Pixel(15));

        var trainer = new OneHotDNN.Trainer(model);
        trainer.LearningRate = 0.1f;
        trainer.Epochs = 1500;
        var reader = new MultiDimensionDataReader(data.text, 0, true);
        trainer.Train(reader.Data_x, reader.Data_y);

        Add(DocRuntime.NewLabel("0~3 Number Predict"));
        Add(DocRuntime.NewTextElement("Hold mouse left to draw\nHole mouse right to erase"));
        var hor = DocRuntime.NewEmptyHorizontal();
        hor.Add(createInputPanel());
        showResult = DocRuntime.NewLabel("Predict : ");
        showResult.style.marginLeft = 30;
        hor.Add(showResult);
        Add(hor);
        Button copyBtn = null;
        copyBtn = DocRuntime.NewButton("Copy to ClipBoard", DocStyle.Current.HintColor, () =>
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 1; i < 26; i++)
                stringBuilder.Append((int)input_x[i]).Append(' ');
            GUIUtility.systemCopyBuffer = stringBuilder.ToString();
            copyBtn.style.backgroundColor = DocStyle.Current.SuccessColor;
            copyBtn.text = "Copied !";
            copyBtn.schedule.Execute(() =>
            {
                copyBtn.style.backgroundColor = DocStyle.Current.HintColor;
                copyBtn.text = "Copy to ClipBoard";
            }).ExecuteLater(800);
        });
        copyBtn.style.width = 165;
        copyBtn.style.marginTop = 15;
        Add(copyBtn);
        input_x[0] = -1;
    }
    FloatVector input_x = new FloatVector(26);
    public void onInputChanged()
    {
        showResult.text = $"Predict : {model.Predict(input_x).IndexOfMaxValue()}";
    }
    public VisualElement createInputPanel()
    {
        int size = 30;
        VisualElement root = DocRuntime.NewEmpty();
        root.style.width = size * 5 + 10;
        root.style.height = size * 5 + 10;
        VisualElement[] visualElements = new VisualElement[25];
        for (int i = 0; i < 25; i++)
        {
            var ve = DocRuntime.NewEmpty();
            visualElements[i] = ve;
            ve.style.width = size;
            ve.style.height = size;
            ve.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            int curi = i+1;
            Action update = () =>
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    input_x[curi] = 1;
                    ve.style.backgroundColor = DocStyle.Current.FrontgroundColor;
                }
                else if (Input.GetKey(KeyCode.Mouse1))
                {
                    input_x[curi] = 0;
                    ve.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                }
                onInputChanged();
            };
            ve.RegisterCallback<MouseMoveEvent>(e => { update(); });
            ve.RegisterCallback<MouseDownEvent>(e => { update(); });
        }
        for(int i = 0; i < 5; i++)
        {
            int beg = i * 5;
            root.Add(DocRuntime.NewHorizontalBar(visualElements[beg], visualElements[beg + 1], visualElements[beg + 2], visualElements[beg + 3], visualElements[beg + 4]));
        }
        foreach(var ve in visualElements)
        {
            ve.style.marginLeft = 2;
            ve.style.marginTop = 2;
        }
        return root;
    }
}
