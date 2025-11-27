using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

public class SceneManager : MonoBehaviour
{
    [SerializeField] CrossBehaviour crossPrefab;
    [SerializeField] RawImage rawImage;
    [SerializeField] TextMeshProUGUI jonesText;
    [SerializeField] TextMeshProUGUI guideText;

    Texture2D texture;
    Vector2 prePos;
    const int Size = 10;

    List<Line> lines = new();
    Line currentLine;
    EdgeBehaviour draggedEdge;
    EdgeBehaviour selectedEdge;

    List<CrossBehaviour> crosses = new();
    CrossBehaviour selectedCross;

    Knot knot;

    void Start()
    {
        texture = new Texture2D(1600, 900, TextureFormat.RGBA32, false);
        rawImage.texture = texture;
        UpdateTexture();
    }

    void FixedUpdate()
    {
        guideText.text = "A: add";

        if (selectedCross)
        {
            guideText.text += "\nS: spin\nD: delete\nF: flip";
        }

        if (selectedEdge && knot != null)
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
            cross.transform.localPosition = Input.mousePosition - new Vector3(800, 450);
            crosses.Add(cross);
        }

        if (selectedEdge && knot != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                knot.ChangeOrientation(selectedEdge.Edge);
                Show();
            }
        }

        if (selectedCross)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                lines.RemoveAll(line => selectedCross.EdgeBehaviours.Any(edge => edge.ConnectedLine == line));
                UpdateTexture();

                selectedCross.Spining = true;
            }            

            if (Input.GetKeyUp(KeyCode.S))
            {
                selectedCross.Spining = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                lines.RemoveAll(line => selectedCross.EdgeBehaviours.Any(edge => edge.ConnectedLine == line));
                UpdateTexture();

                crosses.Remove(selectedCross);
                Destroy(selectedCross.gameObject);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                selectedCross.Flip();
                UpdateTexture();
                knot = null;
            }
        }
    }

    void KnotCheck()
    {
        if (crosses.Count > 0 && lines.Count == 2 * crosses.Count)
        {
            if (knot == null)
            {
                CreateKnot();
            }
        }
        else
        {
            if (knot != null)
            {
                foreach (CrossBehaviour cross in crosses) cross.ResetOrientation();

                knot = null;
                jonesText.text = "Jones: ";
            }
        }
    }

    void CreateKnot()
    {
        for (int i = 0; i < crosses.Count; i++) crosses[i].SetIndex(i);

        knot = new(crosses.Select(cross => cross.CreateCross()).ToList());

        Show();
    }

    void Show()
    {
        foreach (Cross cross in knot.crosses)
        {
            Debug.Log($"{cross.connectedEdges[0]}, {cross.connectedEdges[1]}, {cross.connectedEdges[2]}, {cross.connectedEdges[3]}");
        }
        Polynomial jones = knot.Jones_Polynomial();
        jonesText.text = "Jones: " + jones.String("t");

        foreach (CrossBehaviour cross in crosses) cross.SetOrientation();
    }

    public void OnClickCross(CrossBehaviour selected)
    {
        foreach (CrossBehaviour cross in crosses)
        {
            if (selected != cross) cross.Clear();
        }
        selectedCross = selected;
    }

    public void ClearCross()
    {
        selectedCross = null;
        foreach (CrossBehaviour cross in crosses) cross.Clear();
    }

    public void SetSelectedEdge(EdgeBehaviour edge)
    {
        selectedEdge = edge;
    }

    void UpdateTexture()
    {
        var pixelData = texture.GetPixelData<Color32>(0);

        for (var i = 0; i < pixelData.Length; i++) pixelData[i] = Color.clear;

        foreach (Line line in lines) line.Draw(texture);
        texture.Apply();
    }

    public void OnBeginCrossDrag(CrossBehaviour cross)
    {
        lines.RemoveAll(line => cross.EdgeBehaviours.Any(edge => edge.ConnectedLine == line));
        UpdateTexture();
    }

    public void OnBeginDrag(EdgeBehaviour edge)
    {
        ClearCross();

        draggedEdge = edge;
        currentLine = new();

        lines.RemoveAll(line => edge.ConnectedLine == line);
        UpdateTexture();

        prePos = transform.InverseTransformPoint(edge.transform.position) + new Vector3(800, 450);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Draw(prePos, eventData.position);

        prePos = eventData.position;
    }

    void Draw(Vector2 pos1, Vector2 pos2)
    {
        var dir = 4 * (pos1 - pos2);
        var dist = (int)dir.magnitude;

        dir = 0.25f * dir.normalized;

        for (int d = 0; d < dist; d++)
        {
            var pos = pos2 + dir * d - (Size / 2.0f) * Vector2.one;

            for (int h = 0; h < Size; h++)
            {
                int y = (int)(pos.y + h);
                for (int w = 0; w < Size; w++)
                {
                    int x = (int)(pos.x + w);
                    SetPixel(x, y);
                }
            }
            SetPixel((int)pos.x, (int)pos.y);
        }

        texture.Apply();
    }

    public void OnEndDrag()
    {
        if (draggedEdge && selectedEdge && draggedEdge != selectedEdge)
        {
            lines.RemoveAll(line => selectedEdge.ConnectedLine == line);
            Draw(prePos, transform.InverseTransformPoint(selectedEdge.transform.position) + new Vector3(800, 450));

            lines.Add(currentLine);

            draggedEdge.ConnectedLine = currentLine;
            selectedEdge.ConnectedLine = currentLine;

            draggedEdge.ConnectedEdge = selectedEdge;
            selectedEdge.ConnectedEdge = draggedEdge;
        }

        UpdateTexture();

        draggedEdge = null;
    }

    void SetPixel(int x, int y)
    {
        if (y < 0 || y >= texture.height) return;
        if (x < 0 || x >= texture.width) return;

        texture.SetPixel(x, y, Color.black);
        currentLine.SetPixel((x, y));
    }   
}
