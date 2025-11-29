using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EdgeBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject arrow;

    Image image;
    SceneManager manager;

    public (int, int) Edge { get; set; }
    public EdgeBehaviour ConnectedEdge { get; set; }
    public Line ConnectedLine { get; set; }

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void Init(SceneManager manager)
    {
        this.manager = manager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.red;
        manager.SetPointeredEdge(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.clear;
        manager.SetPointeredEdge(null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        manager.OnBeginEdgeDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        manager.OnEdgeDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        manager.OnEndEdgeDrag();
    }

    public void RemoveConnection()
    {
        if (ConnectedEdge != null)
        {
            ConnectedLine.Remove();
            ConnectedLine = null;
            ConnectedEdge.ConnectedLine = null;
            ConnectedEdge.ConnectedEdge = null;
            ConnectedEdge = null;
        }
    }

    public void SetArrow(bool active)
    {
        arrow.SetActive(active);
    }
}
