using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CardModel card;
    private RectTransform rectTransform;
    private Vector2 offset;
    public bool isSelected;

    public event Action<CardModel> OnBeginDragEvent;
    public event Action<CardModel> OnDragEvent;
    public event Action<CardModel> OnEndDragEvent;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        card = GetComponent<CardModel>();
        isSelected = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnBeginDragEvent?.Invoke(card);
        offset = this.transform.position - new Vector3(eventData.position.x, eventData.position.y, 0);
        isSelected = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position + offset;
        OnDragEvent?.Invoke(card);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragEvent?.Invoke(card);
        isSelected = false;

        // Needs to check if intersecting with player board areas,
        // or if a spell, the center of the screen
        // the connections here need a rework
        if (card.Type == CardType.Unit && !card.Board.CheckValidPlacement(eventData, card))
            return;

        card.Owner.handManager.SetCardPlayState(card, replacedCard: card.Type == CardType.Unit? card.SelectedAreaSlot.Unit : null);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
