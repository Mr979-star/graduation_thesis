using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cross
{
    public List<(int, int)> connectedEdges;
    public Cross((int, int) edge0, (int, int) edge1, (int, int) edge2, (int, int) edge3)
    {
        connectedEdges = new() { edge0, edge1, edge2, edge3 };
    }

    public bool? Track_0to2 { get; private set; } = null;
    public bool? Track_1to3 { get; private set; } = null;

    public void SetTrack(int edgeIndex)
    {
        if (edgeIndex % 2 == 0) Track_0to2 = edgeIndex == 0;
        else Track_1to3 = edgeIndex == 1;
    }

    public int Sign()
    {
        if (Track_0to2 is bool bool1 && Track_1to3 is bool bool2)
        {
            return (bool1 ^ bool2) ? -1 : 1;
        }
        else return 0;
    }

    public Cross Copy() => new(connectedEdges[0], connectedEdges[1], connectedEdges[2], connectedEdges[3]);
}

public class CrossBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] List<EdgeBehaviour> edgeBehaviours;

    Image image;
    RectTransform rectTransform;
    RectTransform parentRectTransform;
    SceneManager manager;

    Color selectedColor = new(1, 0, 0, 0.5f);

    public List<EdgeBehaviour> EdgeBehaviours => edgeBehaviours;
    public int CrossIndex { get; private set; }

    Cross cross;

    public void Init(SceneManager manager)
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent as RectTransform;

        this.manager = manager;

        for(int i = 0; i < 4; i++) edgeBehaviours[i].Init(manager);
    }

    public void SetIndex(int index)
    {
        CrossIndex = index;
        for (int i = 0; i < 4; i++)
        {
            edgeBehaviours[i].Edge = (index, i);
        }
    }

    public Cross CreateCross()
    {
        cross = new(edgeBehaviours[0].ConnectedEdge.Edge, edgeBehaviours[1].ConnectedEdge.Edge, edgeBehaviours[2].ConnectedEdge.Edge, edgeBehaviours[3].ConnectedEdge.Edge);

        return cross;
    }

    public void Clear()
    {
        image.color = Color.clear;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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

    public void SetOrientation()
    {
        edgeBehaviours[0].SetArrow(cross.Track_0to2 == false);
        edgeBehaviours[1].SetArrow(cross.Track_1to3 == false);
        edgeBehaviours[2].SetArrow(cross.Track_0to2 == true);
        edgeBehaviours[3].SetArrow(cross.Track_1to3 == true);
    }

    public void ResetOrientation()
    {
        for (int i = 0; i < 4; i++) edgeBehaviours[i].SetArrow(false);
    }

    public void Flip()
    {
        transform.Rotate(0, 0, 90);

        List<EdgeBehaviour> connections = EdgeBehaviours.Select(edge => edge.ConnectedEdge).ToList();
        List<Line> lines = EdgeBehaviours.Select(edge => edge.ConnectedLine).ToList();

        for (int i = 0; i < 4; i++)
        {
            EdgeBehaviour targetEdge = connections[(i + 1) % 4];
            if (targetEdge && targetEdge.Edge.Item1 == CrossIndex)
            {
                if (targetEdge.Edge.Item2 == i) EdgeBehaviours[i].ConnectedEdge = EdgeBehaviours[(i + 3) % 4];
                else if (targetEdge.Edge.Item2 == (i + 2) % 4) EdgeBehaviours[i].ConnectedEdge = EdgeBehaviours[(i + 1) % 4];
            }
            else
            {
                EdgeBehaviours[i].ConnectedEdge = targetEdge;
            }

            if (EdgeBehaviours[i].ConnectedEdge) EdgeBehaviours[i].ConnectedEdge.ConnectedEdge = EdgeBehaviours[i];
            EdgeBehaviours[i].ConnectedLine = lines[(i + 1) % 4];
        }
    }
}
