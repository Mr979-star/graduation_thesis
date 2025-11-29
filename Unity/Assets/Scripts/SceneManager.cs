using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

public class SceneManager : MonoBehaviour
{
    [SerializeField] RawImage rawImage;
    [SerializeField] CrossBehaviour crossPrefab;
    [SerializeField] TextMeshProUGUI guideText;
    [SerializeField] TextMeshProUGUI jonesText;

    Texture2D texture;
    Map map;
    Vector2 prePos;
    const int DrawSize = 10;

    List<CrossBehaviour> crosses = new();

    Line drawingLine;
    EdgeBehaviour draggingEdge;
    EdgeBehaviour pointeredEdge;
    CrossBehaviour selectedCross;

    Knot knot;

    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        rawImage.texture = texture;
        map = new(texture);

        var pixelData = texture.GetPixelData<Color32>(0);
        for (var i = 0; i < pixelData.Length; i++)
        {
            pixelData[i] = Color.clear;
        }
        texture.Apply();
    }

    void FixedUpdate()
    {
        guideText.text = "A: add";

        if (selectedCross)
        {
            guideText.text += "\nS: spin\nD: delete\nF: flip";
        }

        if (pointeredEdge && knot != null)
        {
            guideText.text += "\nR: reverse";
        }
    }

    void Update()
    {
        KnotCheck();

        if (Input.GetKeyDown(KeyCode.A))
        {
            CrossBehaviour cross = Instantiate(crossPrefab, transform);
            cross.Init(this);
            cross.transform.localPosition = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
            crosses.Add(cross);
        }

        if (selectedCross)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                selectedCross.RemoveConnections();
            }

            if (Input.GetKey(KeyCode.S))
            {
                selectedCross.transform.Rotate(0, 0, 90 * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                selectedCross.RemoveConnections();
                crosses.Remove(selectedCross);
                Destroy(selectedCross.gameObject);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                selectedCross.Flip();
                knot = null;
            }
        }

        if (pointeredEdge && knot != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                knot.ChangeOrientation(pointeredEdge.Edge);
                Calcurate();
            }
        }
    }

    void KnotCheck()
    {
        if (crosses.Count > 0 && crosses.All(cross => cross.EdgeBehaviours.All(edge => edge.ConnectedEdge != null)))
        {
            if (knot == null) CreateKnot();
        }
        else
        {
            if (knot != null)
            {
                foreach (CrossBehaviour cross in crosses)
                {
                    cross.ClearArrow();
                }

                knot = null;
                jonesText.text = "Jones: ";
            }
        }
    }

    void CreateKnot()
    {
        for (int i = 0; i < crosses.Count; i++)
        {
            crosses[i].SetIndex(i);
        }

        knot = new(crosses.Select(cross => cross.CreateCross()).ToList());
        knot.SetOrientation();

        Calcurate();
    }

    void Calcurate()
    {
        try
        {
            jonesText.text = "Jones: " + knot.Jones_Polynomial().String("t");
        }
        catch
        {
            jonesText.text = "Jones: error";
        }

        foreach (CrossBehaviour cross in crosses)
        {
            cross.SetArrow();
        }
    }

    public void OnClickCross(CrossBehaviour cross)
    {
        if (cross != selectedCross)
        {
            ClearSelectedCross();
            selectedCross = cross;
        }
    }

    public void ClearSelectedCross()
    {
        if (selectedCross)
        {
            selectedCross.Clear();
            selectedCross = null;
        }
    }

    public void SetPointeredEdge(EdgeBehaviour edge)
    {
        pointeredEdge = edge;
    }

    public void OnBeginEdgeDrag(EdgeBehaviour edge)
    {
        ClearSelectedCross();
        edge.RemoveConnection();
        draggingEdge = edge;
        drawingLine = new(map);

        prePos = transform.InverseTransformPoint(edge.transform.position) + new Vector3(Screen.width / 2, Screen.height / 2);
    }

    public void OnEdgeDrag(PointerEventData eventData)
    {
        Draw(prePos, eventData.position);
        prePos = eventData.position;
    }

    public void OnEndEdgeDrag()
    {
        if (draggingEdge && pointeredEdge &&
            (draggingEdge.transform.parent != pointeredEdge.transform.parent ||
             Mathf.Abs(draggingEdge.transform.GetSiblingIndex() - pointeredEdge.transform.GetSiblingIndex()) % 2 != 0))
        {
            pointeredEdge.RemoveConnection();
            Draw(prePos, transform.InverseTransformPoint(pointeredEdge.transform.position) + new Vector3(Screen.width / 2, Screen.height / 2));

            draggingEdge.ConnectedLine = drawingLine;
            pointeredEdge.ConnectedLine = drawingLine;
            draggingEdge.ConnectedEdge = pointeredEdge;
            pointeredEdge.ConnectedEdge = draggingEdge;
        }
        else
        {
            drawingLine.Remove();
        }
        draggingEdge = null;
    }

    void Draw(Vector2 pos1, Vector2 pos2)
    {
        var dir = 4 * (pos1 - pos2);
        var dist = (int)dir.magnitude;

        dir = 0.25f * dir.normalized;

        for (int d = 0; d < dist; d++)
        {
            var pos = pos2 + dir * d - (DrawSize / 2.0f) * Vector2.one;
            for (int h = 0; h < DrawSize; h++)
            {
                int y = (int)(pos.y + h);
                for (int w = 0; w < DrawSize; w++)
                {
                    int x = (int)(pos.x + w);
                    SetPixel(x, y);
                }
            }
            SetPixel((int)pos.x, (int)pos.y);
        }
        texture.Apply();
    }

    void SetPixel(int x, int y)
    {
        if (y < 0 || y >= texture.height) return;
        if (x < 0 || x >= texture.width) return;

        texture.SetPixel(x, y, Color.black);
        drawingLine.SetPixel((x, y));
    }   
}
