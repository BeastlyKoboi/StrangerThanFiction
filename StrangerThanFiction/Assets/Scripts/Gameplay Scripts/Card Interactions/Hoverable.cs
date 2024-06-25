using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    public RectTransform rectTransform;
    public Draggable draggable;
    public Vector2 startPos;
    public Vector2 endPos;
    public Quaternion startRotation;
    public Quaternion endRotation;
    public float moveSpeed = 1000;
    public float rotSpeed = 50;

    public bool isHovered;

    private int orderIndex;

    // Update is called once per frame
    void Update()
    {
        if (draggable.isSelected) return;

        if (isHovered)
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, endPos, moveSpeed * Time.deltaTime);
            rectTransform.localRotation = Quaternion.RotateTowards(rectTransform.localRotation, endRotation, rotSpeed * Time.deltaTime);
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, startPos, moveSpeed * Time.deltaTime);
            rectTransform.localRotation = Quaternion.RotateTowards(rectTransform.localRotation, startRotation, rotSpeed * Time.deltaTime);
        }
    }
    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        draggable = GetComponent<Draggable>();
        startPos = rectTransform.anchoredPosition;
        endPos = new Vector2(startPos.x, startPos.y + 150);
        startRotation = rectTransform.localRotation;
        endRotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isHovered = true;
        orderIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {

    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isHovered = false;
        transform.SetSiblingIndex(orderIndex);
    }
}
