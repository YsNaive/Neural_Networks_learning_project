using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    [System.Serializable]
    public class CarDNN : TrainableModel<FloatVector, FloatVector>
    {
        // input dim, hiddens... , output dim
        public CarDNN(params int[] layers)
        {
            if (layers.Length < 2)
                throw new InputLengthNotMatchException($"Must have more then 2 input while create DNN as in/out dimension.");
            Layers = new Dense[layers.Length - 1];
            for (int i = 1, imax = layers.Length; i < imax; i++)
            {
                Layers[i - 1] = new Dense(layers[i], layers[i - 1], -1);
            }
            foreach (var l in Layers)
                l.ActivationFunction = new AF_ReLU();
        }
        public Dense[] Layers;
        public override ModelResult Eval(FloatVector[] predicts, FloatVector[] answers)
        {
            ModelResult result = new ModelResult();
            int dataSize = predicts.Length;
            float loss = 0;
            for (int i = 0; i < dataSize; i++)
                loss += Mathf.Abs(predicts[i][0] - answers[i][0]);
            result.Acc = 0;
            result.Loss = loss/dataSize;
            return result;
        }

        public override FloatVector Predict(FloatVector input)
        {
            FloatVector current = input;
            foreach (var layer in Layers)
            {
                current = layer.Predict(current, layer != Layers[^1]);
            }
            return current;
        }

        public override bool Train(FloatVector input, FloatVector label, float lr)
        {
            List<FloatVector> predicts = new List<FloatVector>();
            FloatVector current = input;
            foreach (var layer in Layers)
            {
                current = layer.Predict(current, true);
                predicts.Add(current);
            }

            Dictionary<Vector2, float> gdTable = new Dictionary<Vector2, float>();
            bool isOutputLayer = true;
            for (int i = Layers.Length - 1; i >= 0; i--)
            {
                for (int j = 0, jmax = Layers[i].NeuralsCount; j < jmax; j++)
                {
                    float gd = 0;

                    float y = predicts[i][j + 1];
                    if (isOutputLayer)
                    {
                        gd = (label[j] - y) * Layers[i].ActivationFunction.Differential(y);
                    }
                    else
                    {
                        float sum = 0;
                        for (int k = 0, kmax = Layers[i + 1].NeuralsCount; k < kmax; k++)
                        {
                            sum += gdTable[new Vector2(i + 1, k)] * Layers[i + 1].Neurals[k].Weights[j + 1];
                        }
                        gd = sum * Layers[i].ActivationFunction.Differential(y);
                    }
                    gdTable.Add(new Vector2(i, j), gd);
                }
                isOutputLayer = false;
            }
            for (int i = 0, imax = Layers.Length; i < imax; i++)
            {
                FloatVector xs = (i == 0) ? input : predicts[i - 1];

                for (int j = 0, jmax = Layers[i].NeuralsCount; j < jmax; j++)
                {
                    float dw = gdTable[new Vector2(i, j)];
                    dw *= lr;
                    FloatVector delta = new FloatVector(Layers[i].Neurals[j].Weights.Length);
                    for (int k = 0, kmax = Layers[i].Neurals[j].Weights.Length; k < kmax; k++)
                    {
                        delta[k] = dw * xs[k];
                    }
                    Layers[i].Neurals[j].Weights.Add(delta);
                }
            }
            return true;
        }
    }
    float SteeringLimitDeg = 40;
    public float SteeringDeg = 0;
    public float FacingDeg
    {
        get => m_FacingDeg;
        set
        {
            transform.eulerAngles = new Vector3(0, 0, value - 90);
            m_FacingDeg = value;
        }
    }
    float m_FacingDeg = 0;
    public bool IsCrash => isCrash;
    bool isCrash = false;
    public CarDNN Brain = new CarDNN(3, 4, 1);
    public CarDNN.Trainer Trainer;
    public SpriteRenderer CarRenderer;
    public GameObject SteeringRender;
    private void Start()
    {
        Senser4D[0] = -1;
        Senser6D[0] = -1;
    }

    [NonSerialized] public FloatVector Senser4D = new FloatVector(4);
    [NonSerialized] public FloatVector Senser6D = new FloatVector(6);
    public LineRenderer SenserForwardRenderer;
    public LineRenderer SenserLeftRenderer;
    public LineRenderer SenserRightRenderer;
    public LineRenderer PathRenderer;

    private void Update()
    {
        Vector2 org = transform.position;
        foreach (var group in new List<(int index, LineRenderer renderer, float direction)>()
        {(1, SenserForwardRenderer, 0f), (2, SenserRightRenderer, -45f), (3, SenserLeftRenderer, 45f) })
        {
            var directionRad = (FacingDeg + 90 + group.direction) * Mathf.Deg2Rad;
            var dir = new Vector2(Mathf.Sin(directionRad),- Mathf.Cos(directionRad));
            var cast = Physics2D.Raycast(org, dir, 1000, LayerMask.GetMask("Border"));
            group.renderer.SetPosition(0, org);
            group.renderer.SetPosition(1, cast.point);

            Senser4D[group.index] = cast.distance;
            Senser6D[group.index + 2] = cast.distance;
        }
        Senser6D[1] = org.x;
        Senser6D[2] = org.y;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Border"))
        {
            isCrash = true;
            CarRenderer.color = DocStyle.Current.DangerColor;
        }
        else if (collision.collider.CompareTag("EndZone"))
        {
            isCrash = true;
            CarRenderer.color = DocStyle.Current.SuccessColor;
        }
    }
    public List<List<float>> PathLog = new();
    public void Next()
    {
        if (isCrash)
            return;
        var is4D = Brain.Layers[0].Neurals[0].Weights.Length == Senser4D.Length;
        var vector = is4D ? Senser4D : Senser6D;
        var log = new List<float>((is4D ? Senser4D : Senser6D).Values) { SteeringDeg };
        PathLog.Add(log);
        SteeringDeg = Brain.Predict(vector)[0]-40;
        SteeringDeg = Mathf.Clamp(SteeringDeg, -SteeringLimitDeg, SteeringLimitDeg);
        var facingRad = m_FacingDeg * Mathf.Deg2Rad;
        var directionRad = SteeringDeg * Mathf.Deg2Rad;
        var nextFacingRad = facingRad - Mathf.Asin(2f * Mathf.Sin(directionRad) / 6f);
        var nextX = transform.position.x + (Mathf.Cos(facingRad + directionRad) + Mathf.Sin(directionRad)*Mathf.Sin(facingRad));
        var nextY = transform.position.y + (Mathf.Sin(facingRad + directionRad) - Mathf.Sin(directionRad)*Mathf.Cos(facingRad));
        transform.position = new Vector3(nextX, nextY);
        FacingDeg = nextFacingRad * Mathf.Rad2Deg;
        pathPositions.Add(new Vector3(nextX, nextY,0));
        PathRenderer.positionCount = pathPositions.Count;
        PathRenderer.SetPositions(pathPositions.ToArray());
        SteeringRender.transform.localEulerAngles = new Vector3(0, 0, -SteeringDeg);
    }

    Vector2 initPos;
    float initFacingDeg;
    public void Init(Vector2 pos, float facingDeg)
    {
        initPos = pos;
        initFacingDeg = facingDeg;
        ResetState();
    }
    List<Vector3> pathPositions = new();
    public void ResetState()
    {
        transform.position = initPos;
        FacingDeg = initFacingDeg;
        isCrash = false;
        CarRenderer.color = DocStyle.Current.FrontgroundColor;
        pathPositions.Clear();
        PathLog.Clear();
        pathPositions.Add(initPos);
        PathRenderer.positionCount = pathPositions.Count;
        PathRenderer.SetPositions(pathPositions.ToArray());
    }
   
}
