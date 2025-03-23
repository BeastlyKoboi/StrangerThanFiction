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

    public event Action<CardModel> OnLeftClick;
    public event Action<CardModel> OnRightClick;

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
        if (Vector3.Distance(downPos, eventData.position) < 7)
        {
            OnClickWithoutDrag?.Invoke(GetComponent<CardModel>());

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick?.Invoke(GetComponent<CardModel>());
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick?.Invoke(GetComponent<CardModel>());
            }
        }
    }

    public void SetOnClickWithoutDrag(Action<CardModel> action)
    {
        OnClickWithoutDrag = action;
    }
}