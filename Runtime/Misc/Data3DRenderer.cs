using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data3DRenderer : MonoBehaviour
{
    public static Data3DRenderer Instance;
    public Camera RenderCamera;
    public RenderTexture RenderTexture;
    public Vector3 PosOffset = Vector3.zero;
    public Transform UnitRoot;
    public GameObject Unit;

    private void Awake()
    {
        Instance = this;
        RenderTexture = new RenderTexture(200, 200, 24);
        RenderCamera.targetTexture = RenderTexture;
    }
    Color[] colors = new Color[] { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, new Color(.7f, .7f, .4f) };
    public void Render(MultiDimensionDataReader reader)
    {
        for (int j = UnitRoot.childCount - 1; j >= 0; j--) 
            Destroy(UnitRoot.GetChild(j).gameObject);
        UnitRoot.DetachChildren();
        if (reader.DimensionX != 4) return;
        int i = 0;
        foreach(var x in reader.Data_x)
        {
            var obj =Instantiate(Unit, new Vector3(x[1]-0.5f, x[2] - 0.5f, x[3] - 0.5f), new Quaternion(), UnitRoot.transform);
            var mat = obj.GetComponent<MeshRenderer>();
            mat.material = new Material(mat.material);
            mat.material.color = colors[reader.Data_y[i++].IndexOfMaxValue()];
        }
    }
}
