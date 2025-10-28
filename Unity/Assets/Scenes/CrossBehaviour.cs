using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[Serializable] public class Edge
{
    public int crossIndex;
    public int edgeIndex;
     
    public Edge(int crossIndex, int edgeIndex) { this.crossIndex = crossIndex; this.edgeIndex = edgeIndex; }
    public Edge Copy() => new(crossIndex, edgeIndex);

    public string String()
    {
        return $"[{crossIndex}, {edgeIndex}]";
    }    
}

public class Cross
{
    public List<Edge> connectedEdges = new();

    public Cross(List<Edge> connectedEdges) { this.connectedEdges = connectedEdges; }
    public Cross(params Edge[] connectedEdges) { this.connectedEdges = connectedEdges.ToList(); }
    public Cross Copy() => new(new List<Edge>(connectedEdges));

    public Edge ConnectedEdge(int edgeIndex) => connectedEdges[edgeIndex];

    public void Print()
    {
        string log = "";

        foreach (Edge edge in connectedEdges) log += edge.String();

        Debug.Log(log);
    }
}

public class CrossBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] List<EdgeBehaviour> edgeBehaviours;

    Image image;
    RectTransform rectTransform;
    RectTransform parentRectTransform;
    SceneManager manager;

    Color selectedColor = new(1, 0, 0, 0.5f);
    Color NormalColor = new(1, 1, 1, 0.1f);

    public List<EdgeBehaviour> EdgeBehaviours => edgeBehaviours;

    public void Init(SceneManager manager)
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent as RectTransform;

        this.manager = manager;

        for(int i = 0; i < 4; i++) edgeBehaviours[i].Init(manager, i);
    }

    public void SetIndex(int index)
    {
        foreach (EdgeBehaviour edge in edgeBehaviours) edge.crossIndex = index;
    }

    public void Clear()
    {
        image.color = NormalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        foreach (EdgeBehaviour edge in edgeBehaviours)
        {
            if (edge.Pointered) return;
        }

        image.color = selectedColor;
        manager.OnClickCross(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnPointerDown(eventData);
        manager.OnBeginCrossDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, Camera.main, out Vector2 localPosition);

        rectTransform.anchoredPosition = localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void SetOrientation(bool isOut, int edgeIndex, int trackIndex)
    {
        edgeBehaviours[edgeIndex].SetArrow(isOut, trackIndex);
    }

    public void ResetOrientation()
    {
        for (int i = 0; i < 4; i++) edgeBehaviours[i].ResetArrow();
    }

    public int Sgn()
    {
        if (edgeBehaviours[0].IsOut && edgeBehaviours[1].IsOut) return 1;
        if (edgeBehaviours[0].IsOut && !edgeBehaviours[1].IsOut) return -1;
        if (!edgeBehaviours[0].IsOut && edgeBehaviours[1].IsOut) return -1;
        if (!edgeBehaviours[0].IsOut && !edgeBehaviours[1].IsOut) return 1;

        return 0;
    }
}
