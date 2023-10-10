using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextAsset Data;
    public DNN DNN = new DNN(2 ,4, 2);
    public List<List<float>> x = new List<List<float>>();
    public List<List<float>> y = new List<List<float>>();
    void Start()
    {
        foreach (var item in Data.text.Split('\n'))
        {
            var vals = item.Split(" ");
            if (vals.Length != 3) continue;
            var p = new Vector3(float.Parse(vals[0]), float.Parse(vals[1]), (int.Parse(vals[2]) == 1)? 1:-1);
            x.Add(new List<float> { -1, ActivationFunction.Sigmoid(p.x), ActivationFunction.Sigmoid(p.y) });
            y.Add((p.z == 1)? new List<float> { 1, 0 }: new List<float> { 0, 1 });
        }
        for(int i = 0; i < DNN.Epochs; i++)
            DNN.Train(x, y);
        var pre = DNN.Predict(x);
        int correct = 0;
        for(int i=0,imax = pre.Length; i < imax; i++)
        {
            Debug.Log(pre[i][0]+","+pre[i][1] + " vs " + new Vector2(y[i][0], y[i][1]));
            if ((pre[i][0] > pre[i][1]) == (y[i][0]==1))
            {
                correct++;
            }
        }
        Debug.Log(correct + "/" + pre.Length);
        //Debug.Log(DNN.Predict(new List<float>() { 1, 1, 1, 1 })[0]);
    }
}
