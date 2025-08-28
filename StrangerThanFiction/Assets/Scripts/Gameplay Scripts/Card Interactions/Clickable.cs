using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    private ISelectable selectable;
    private Vector2 downPos;
        
    public float requiredHoldTime = .75f; // Time in seconds for a long click
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;

    public event Action<ISelectable> OnClickWithoutDrag;
    public event Action OnDoubleClick;

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

    public void SetOnLeftClick(Action<ISelectable> action)
    {
        OnLeftClick = action;
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
            if (selectable != null)
                OnLongClick?.Invoke(selectable);
        }
    }

    public void SetSelectableTarget(ISelectable target)
    {
        selectable = target;
    }
}