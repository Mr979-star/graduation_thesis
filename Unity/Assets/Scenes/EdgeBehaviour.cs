using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EdgeBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public int edgeIndex;
    [HideInInspector] public int crossIndex;
    [SerializeField] GameObject outArrow;
    [SerializeField] GameObject inArrow;

    public bool IsOut => outArrow.activeSelf;
    public bool Pointered => image.color == Color.red;

    Image image;
    SceneManager manager;

    Color NormalColor = new(1, 1, 1, 0f);

    bool dragging = false;
    int trackIndex = -1;
    public int TrackIndex => trackIndex;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void Init(SceneManager manager, int edgeIndex)
    {
        this.manager = manager; this.edgeIndex = edgeIndex;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.red;
        manager.ConnectEdge(this);
        manager.SetSelectedEdge(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dragging) image.color = NormalColor;

        manager.SetSelectedEdge(null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        manager.OnBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        manager.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        image.color = NormalColor;
        manager.OnEndDrag();
    }

    public void SetArrow(bool isOut, int trackIndex)
    {
        outArrow.SetActive(isOut);
        inArrow.SetActive(!isOut);
        this.trackIndex = trackIndex;
    }

    public void ResetArrow()
    {
        outArrow.SetActive(false);
        inArrow.SetActive(false);
    }
}
