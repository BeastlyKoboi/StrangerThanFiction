using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Clickable : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    private ISelectable selectable;
    private Vector2 downPos;
        
    public float requiredHoldTime = .75f; // Time in seconds for a long click
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;
    private bool isBeingDragged = false;

    public event Action<ISelectable> OnClickWithoutDrag;
    public event Action<ISelectable> OnDoubleClick;

    public event Action<ISelectable> OnLeftClick;
    public event Action<ISelectable> OnRightClick;
    public event Action<ISelectable> OnLongClick;

    private void Awake()
    {
        if (selectable == null)
            selectable = GetComponent<ISelectable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) OnDoubleClick?.Invoke(selectable);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
        downPos = eventData.position;
        StartCoroutine(CheckLongClick()); // Start checking for long click
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        isBeingDragged = false;

        if (Vector3.Distance(downPos, eventData.position) < 7)
        {
            if (selectable == null) return;

            OnClickWithoutDrag?.Invoke(selectable);

            if (eventData.button == PointerEventData.InputButton.Left)
                OnLeftClick?.Invoke(selectable);

            if (eventData.button == PointerEventData.InputButton.Right)
                OnRightClick?.Invoke(selectable);
        }
    }

    public void SetOnClickWithoutDrag(Action<ISelectable> action)
    {
        OnClickWithoutDrag = action;
    }

    public void SetOnDoubleClick(Action<ISelectable> action)
    {
        OnDoubleClick = action;
    }

    public void SetOnLeftClick(Action<ISelectable> action)
    {
        OnLeftClick = action;
    }

    private IEnumerator CheckLongClick()
    {
        while (isPointerDown && pointerDownTimer < requiredHoldTime && !isBeingDragged)
        {
            pointerDownTimer += Time.deltaTime;
            yield return null; // Wait for next frame
        } 

        if (isPointerDown && pointerDownTimer >= requiredHoldTime && !isBeingDragged)
        {
            if (selectable != null)
                OnLongClick?.Invoke(selectable);
        }
    }

    public void SetSelectableTarget(ISelectable target)
    {
        selectable = target;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isBeingDragged = true;
    }
}