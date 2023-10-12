using NaiveAPI.DocumentBuilder;
using NaiveAPI.Runtime_Window;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ModelTrainer<Tin, Tout>
{
    public ModelTrainer(TrainableModel<Tin, Tout> model)
    {
        Model = model;
    }

    public int Epochs = 10;
    public float LearningRate = 0.01f;
    public float EarlyStopAcc = 1f;
    public TrainableModel<Tin, Tout> Model
    {
        get => m_model;
        set
        {
            m_model = value;
            Reset();
        }
    }
    public TrainableModel<Tin, Tout> m_model;
    public List<ModelResult> TrainHistory = new List<ModelResult>();
    public List<ModelResult> ValHistory = new List<ModelResult>();
    public event Action<ModelResult> TrainStepCallback;
    public void Reset()
    {
        TrainHistory.Clear();
        ValHistory.Clear();
    }
    public void Train(List<Tin> train_x, List<Tout> train_y, List<Tin> val_x = null, List<Tout> val_y = null)
    {
        if (TrainHistory.Count != 0)
        {
            if (TrainHistory[^1].Acc >= EarlyStopAcc)
                return;
        }
        for(int i = 0; i < Epochs; i++)
        {
            TrainHistory.Add(Model.Train(train_x, train_y, LearningRate));
            if (val_x != null && val_y != null)
                ValHistory.Add(Model.Eval(Model.Predict(val_x), val_y.ToArray()));
            TrainStepCallback?.Invoke(TrainHistory[^1]);
            if (TrainHistory[^1].Acc >= EarlyStopAcc) return;
        }
    }
    public VisualElement CreateEditVisual()
    {
        VisualElement root = DocRuntime.NewEmpty();
        var lr = (FloatDrawer)RuntimeDrawer.CreateDrawer("Learning Rate", LearningRate);
        lr.OnValueChanged += () => { LearningRate = lr.value; };
        var epochs = (IntDrawer)RuntimeDrawer.CreateDrawer("Epochs", Epochs);
        epochs.OnValueChanged += () => { Epochs = epochs.value; };
        var earlyStop = (FloatDrawer)RuntimeDrawer.CreateDrawer("EarlyStop acc", EarlyStopAcc);
        earlyStop.OnValueChanged += () => { EarlyStopAcc = earlyStop.value; };
        root.Add(lr);
        root.Add(epochs);
        root.Add(earlyStop);
        return root;
    }
}