using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    private Vector2 downPos;

    public float requiredHoldTime = .75f; // Time in seconds for a long click
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;

    public event Action<CardModel> OnClickWithoutDrag;
    public event Action OnDoubleClick;

    public event Action<CardModel> OnLeftClick;
    public event Action<CardModel> OnRightClick;
    public event Action<CardModel> OnLongClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) OnDoubleClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
        StartCoroutine(CheckLongClick()); // Start checking for long click
        downPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;

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

    private IEnumerator CheckLongClick()
    {
        while (isPointerDown && pointerDownTimer < requiredHoldTime)
        {
            pointerDownTimer += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        if (isPointerDown && pointerDownTimer >= requiredHoldTime)
        {
            OnLongClick?.Invoke(GetComponent<CardModel>()); // Trigger the long click event
        }
    }
}