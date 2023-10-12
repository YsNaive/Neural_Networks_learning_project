using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    public TextAsset Data;
    public OneHotDNN DNN = new OneHotDNN(2 ,4, 2);
    public int epochs = 10;

    void Start()
    {
        var data = new MultiDimensionDataReader(Data.text, 0);
        data.MinMaxNormalize();
        FindAnyObjectByType<Data3DRenderer>().Render(data);
        //var root = GetComponent<UIDocument>().rootVisualElement;
        //VisualElement ve = new VisualElement();
        //ve.style.width = 200;
        //ve.style.height = 200;
        //ve.style.position = Position.Absolute;
        ////root.Add(ve);
        //ModelHistoryPainter painter = new ModelHistoryPainter(50, 50);



        //MultiDimensionDataReader reader = new MultiDimensionDataReader(Data.text,0.2f);

        //ModelTrainer<FloatVector, FloatVector> trainer = new ModelTrainer<FloatVector, FloatVector>(DNN);
        //trainer.Epochs = epochs;
        //trainer.LearningRate = 0.0001f;
        //trainer.TrainStepCallback += (his) => { Debug.Log(his); };
        //trainer.Train(reader.Train_x, reader.Train_y, reader.Val_x, reader.Val_y);

        //painter.DrawHistory(trainer.TrainHistory);
        //ve.style.backgroundImage = painter.Texture;
    }
}
