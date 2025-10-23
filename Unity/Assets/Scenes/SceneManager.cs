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
    EdgeBehaviour connectedEdge;
    EdgeBehaviour selectedEdge;

    List<CrossBehaviour> crosses = new();
    CrossBehaviour selectedCross;

    bool spining = false;

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
                knot.ChangeOrientation(selectedEdge.TrackIndex);
                Show();
            }
        }

        if (selectedCross)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                lines.RemoveAll(x => x.ConnectWith(selectedCross));
                UpdateTexture();

                spining = true;
            }            

            if (spining)
            {
                selectedCross.transform.Rotate(0, 0, 90 * Time.deltaTime);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                spining = false;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                lines.RemoveAll(x => x.ConnectWith(selectedCross));
                UpdateTexture();

                crosses.Remove(selectedCross);
                Destroy(selectedCross.gameObject);

                spining = false;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                selectedCross.transform.Rotate(0, 0, 90);
                SwapCross(selectedCross);
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

        List<Cross> crossList = new();

        for (int i = 0; i < crosses.Count; i++)
        {
            List<Edge> edgeList = new() { null, null, null, null };

            foreach (Line line in lines)
            {
                if (line.Edge1.crossIndex == i) edgeList[line.Edge1.edgeIndex] = new(line.Edge2.crossIndex, line.Edge2.edgeIndex);
                if (line.Edge2.crossIndex == i) edgeList[line.Edge2.edgeIndex] = new(line.Edge1.crossIndex, line.Edge1.edgeIndex);
            }
            Cross cross = new(edgeList);
            crossList.Add(cross);
        }

        knot = new(crossList);
        Show();
    }

    void Show()
    {
        knot.Print();

        for (int i = 0; i < knot.edgeTracks.Count; i++)
        {
            for (int j = 0; j < knot.edgeTracks[i].Count; j++)
            {
                crosses[knot.edgeTracks[i][j].crossIndex].SetOrientation(j % 2 == 0, knot.edgeTracks[i][j].edgeIndex, i);
            }
        }

        knot.Writhe = crosses.Sum(cross => cross.Sgn());

        Polynomial jones = knot.Jones_Polynomial();
        jonesText.text = "Jones: " + jones.String("t");
    }

    void SwapCross(CrossBehaviour cross)
    {
        List<Line> targetLines = new() { null, null, null, null };
        List<EdgeBehaviour> targetEdges = new() { null, null, null, null };

        foreach (Line line in lines)
        {
            for (int i = 0; i < 4; i++)
            {
                if (line.Edge1 == cross.EdgeBehaviours[i]) { targetLines[i] = line; targetEdges[i] = line.Edge1; }
                else if (line.Edge2 == cross.EdgeBehaviours[i]) { targetLines[i] = line; targetEdges[i] = line.Edge2; }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (targetLines[i] != null) targetLines[i].ChangeEdge(targetEdges[i], cross.EdgeBehaviours[(i + 3) % 4]);
        }
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

    public void ConnectEdge(EdgeBehaviour edge)
    {
        if (draggedEdge == null) return;

        connectedEdge = edge;
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
        lines.RemoveAll(x => x.ConnectWith(cross));
        UpdateTexture();
    }

    public void OnBeginDrag(EdgeBehaviour edge)
    {
        ClearCross();

        draggedEdge = edge;
        currentLine = new();

        lines.RemoveAll(x => x.ConnectWith(edge));
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
        if (draggedEdge && connectedEdge)
        {
            lines.RemoveAll(x => x.ConnectWith(connectedEdge));
            Draw(prePos, transform.InverseTransformPoint(connectedEdge.transform.position) + new Vector3(800, 450));

            currentLine.Set(draggedEdge, connectedEdge);
            lines.Add(currentLine);
        }

        UpdateTexture();

        draggedEdge = null; connectedEdge = null;
    }

    void SetPixel(int x, int y)
    {
        if (y < 0 || y >= texture.height) return;
        if (x < 0 || x >= texture.width) return;

        texture.SetPixel(x, y, Color.black);
        currentLine.SetPixel((x, y));
    }   
}
