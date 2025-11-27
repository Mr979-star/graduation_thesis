using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EdgeBehaviour : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject outArrow;
    [SerializeField] GameObject inArrow;

    Image image;
    SceneManager manager;

    Color NormalColor = new(1, 1, 1, 0f);

    public (int, int) Edge;
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
        manager.SetSelectedEdge(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = NormalColor;

        manager.SetSelectedEdge(null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        manager.OnBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        manager.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.color = NormalColor;
        manager.OnEndDrag();
    }

    public void SetArrow(bool isOut)
    {
        outArrow.SetActive(isOut);
        inArrow.SetActive(!isOut);
    }

    public void ResetArrow()
    {
        outArrow.SetActive(false);
        inArrow.SetActive(false);
    }
}
