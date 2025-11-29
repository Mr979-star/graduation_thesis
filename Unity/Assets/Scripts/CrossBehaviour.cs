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

    readonly Color SelectedColor = new(1, 0, 0, 0.5f);
    
    Cross cross;

    public List<EdgeBehaviour> EdgeBehaviours => edgeBehaviours;

    public void Init(SceneManager manager)
    {
        image = GetComponent<Image>();
        rectTransform = transform as RectTransform;
        parentRectTransform = rectTransform.parent as RectTransform;

        this.manager = manager;

        for(int i = 0; i < 4; i++) EdgeBehaviours[i].Init(manager);

        OnPointerDown(null);
    }

    public void SetIndex(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            EdgeBehaviours[i].Edge = (index, i);
        }
    }

    public Cross CreateCross()
    {
        cross = new(EdgeBehaviours[0].ConnectedEdge.Edge, EdgeBehaviours[1].ConnectedEdge.Edge,
                    EdgeBehaviours[2].ConnectedEdge.Edge, EdgeBehaviours[3].ConnectedEdge.Edge);

        return cross;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = SelectedColor;
        manager.OnClickCross(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RemoveConnections();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, eventData.position, Camera.main, out Vector2 localPosition);
        rectTransform.anchoredPosition = localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void Clear()
    {
        image.color = Color.clear;
    }

    public void RemoveConnections()
    {
        foreach (EdgeBehaviour edge in EdgeBehaviours)
        {
            edge.RemoveConnection();
        }
    }

    public void SetArrow()
    {
        EdgeBehaviours[0].SetArrow(cross.Track_0to2 == false);
        EdgeBehaviours[1].SetArrow(cross.Track_1to3 == false);
        EdgeBehaviours[2].SetArrow(cross.Track_0to2 == true);
        EdgeBehaviours[3].SetArrow(cross.Track_1to3 == true);
    }

    public void ClearArrow()
    {
        for (int i = 0; i < 4; i++) EdgeBehaviours[i].SetArrow(false);
    }

    public void Flip()
    {
        transform.Rotate(0, 0, 90);

        List<EdgeBehaviour> connections = EdgeBehaviours.Select(edge => edge.ConnectedEdge).ToList();
        List<Line> lines = EdgeBehaviours.Select(edge => edge.ConnectedLine).ToList();

        for (int i = 0; i < 4; i++)
        {
            EdgeBehaviour targetEdge = connections[(i + 1) % 4];

            if (targetEdge == EdgeBehaviours[i])
            {
                EdgeBehaviours[i].ConnectedEdge = EdgeBehaviours[(i + 3) % 4];
            }
            else if (targetEdge == EdgeBehaviours[(i + 2) % 4])
            {
                EdgeBehaviours[i].ConnectedEdge = EdgeBehaviours[(i + 1) % 4];
            }
            else
            {
                EdgeBehaviours[i].ConnectedEdge = targetEdge;
            }

            if (EdgeBehaviours[i].ConnectedEdge)
            {
                EdgeBehaviours[i].ConnectedEdge.ConnectedEdge = EdgeBehaviours[i];
            }

            EdgeBehaviours[i].ConnectedLine = lines[(i + 1) % 4];
        }

        ClearArrow();
    }
}
