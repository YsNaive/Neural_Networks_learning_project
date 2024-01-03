using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public TextAsset MapData;
    public Transform BorderRoot;
    public GameObject BorderUnit;
    public GameObject EndZone;
    public CarController CarController;
    public float BorderWidth;
    private void Start()
    {
        var datas = MapData.text.Split('\n');
        var positionCount = datas.Length - 3;
        var posStr = datas[0].Split(',');
        var carPosition = new Vector2(float.Parse(posStr[0]), float.Parse(posStr[1]));
        var carFacing = float.Parse(posStr[2]);
        Vector2[] positions = new Vector2[positionCount];
        for (int i = 0;  i < positionCount; i++)
        {
            posStr = datas[i+3].Split(',');
            positions[i] = new Vector2(float.Parse(posStr[0]), float.Parse(posStr[1]));
        }
        LoadBorder(positions);
        CarController.Init(carPosition, carFacing);
        posStr = datas[1].Split(',');
        var endPoint1 = new Vector2(float.Parse(posStr[0]), float.Parse(posStr[1]));
        posStr = datas[2].Split(',');
        var endPoint2 = new Vector2(float.Parse(posStr[0]), float.Parse(posStr[1]));
        var endPosition = (endPoint1 + endPoint2) / 2f;
        var endScale = endPoint1 - endPoint2;
        endScale.x = Mathf.Abs(endScale.x) + BorderWidth;
        endScale.y = Mathf.Abs(endScale.y) + BorderWidth;
        EndZone.transform.position = endPosition;
        EndZone.transform.localScale = endScale;
    }

    public void LoadBorder(Vector2[] positions)
    {
        for (int i = 0; i < positions.Length - 1; i++)
        {
            var obj = Instantiate(BorderUnit, BorderRoot);
            obj.SetActive(true);
            var trans = obj.transform;
            trans.position = ((positions[i] + positions[i + 1]) / 2f);
            var scale = positions[i] - positions[i + 1];
            scale.x = Mathf.Abs(scale.x) + BorderWidth;
            scale.y = Mathf.Abs(scale.y) + BorderWidth;
            if (scale.x < scale.y)
                obj.GetComponent<BoxCollider2D>().size = new Vector2(0.1f, 1f);
            else
                obj.GetComponent<BoxCollider2D>().size = new Vector2(1f, 0.1f);
            trans.localScale = scale;
        }
    }
}
