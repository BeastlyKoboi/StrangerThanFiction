using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    private Vector2 downPos;

    public event Action<CardModel> OnClickWithoutDrag;
    public event Action OnDoubleClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) OnDoubleClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        downPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (downPos == eventData.position)
        {
            OnClickWithoutDrag?.Invoke(GetComponent<CardModel>());
        }
    }

    public void SetOnClickWithoutDrag(Action<CardModel> action)
    {
        OnClickWithoutDrag = action;
    }
}