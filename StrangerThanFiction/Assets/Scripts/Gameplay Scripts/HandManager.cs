using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    private UIManager uiManager;
    public CardPile Hand { get; set; }

    public int NumPlayableCards = 0;

    public Vector2 handBounds = new Vector2(500, 20);
    public int rotationBounds = 20;
    public int cardGap = 150;
    public int cardDeviation = 60;
    public Vector2 handCenter = new Vector2(0, -500);

    public CardModel hoveredCard; 
    public CardModel selectedCard;

    public CardPlayState PlayState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        Hand = new CardPile();
    }

    public void AddCardToHandFromDeck(CardModel card)
    {
        if (!card.GetComponent<Appear>()) card.gameObject.AddComponent<Appear>();
        if (!card.GetComponent<Hoverable>()) card.gameObject.AddComponent<Hoverable>();
        if (!card.GetComponent<Draggable>()) card.gameObject.AddComponent<Draggable>();

        card.transform.SetParent(transform);
        Hand.Insert(0, card);
        UpdateTargetTransforms();
    }

    public void RemoveCardFromHand(CardModel card)
    {
        Destroy(card.GetComponent<Appear>());
        Destroy(card.GetComponent<Hoverable>());
        Destroy(card.GetComponent<Draggable>());
        Hand.Remove(card);
        UpdateTargetTransforms();
    }

    public void SetCardPlayState(
        CardModel card,
        List<CardModel> allyUnitTargets = null,
        List<CardModel> enemyUnitTargets = null,
        List<CardModel> allyCardTargets = null,
        List<CardModel> enemyCardTargets = null,
        CardModel replacedCard = null)
    {
        if (card != null)
            PlayState = new CardPlayState(card, 
                allyUnitTargets: allyUnitTargets,
                enemyUnitTargets: enemyUnitTargets,
                allyCardTargets: allyCardTargets,
                enemyCardTargets: enemyCardTargets,
                replacedCard: replacedCard);
        else 
            PlayState = null;
    }

    private void UpdateTargetTransforms()
    {
        float startingXPos = -(Hand.Count - 1) * cardGap / 2f;

        for (int cardIndex = 0; cardIndex < Hand.Count; cardIndex++)
        {
            Vector2 cardPos = new Vector2(startingXPos + cardIndex * cardGap, 
                handCenter.y + CalculateCardYPos(cardIndex, Hand.Count, cardDeviation));
            float zRotation = CalculateCardRotation(cardIndex, Hand.Count, rotationBounds);

            Hand[cardIndex].GetComponent<Appear>().RefreshTarget(cardPos, zRotation);
            Hand[cardIndex].GetComponent<Hoverable>().rectTransform.SetAsLastSibling();
        }

    }

    private float CalculateCardRotation(int cardIndex, int totalCount, int maxRotation)
    {
        if (totalCount <= 1) return 0;
        float middleIndex = (totalCount - 1) / 2f;
        return -(cardIndex - middleIndex) * (2 * maxRotation) / (totalCount - 1);
    }

    private float CalculateCardYPos(int cardIndex, int totalCount, int maxDeviation)
    {
        if (totalCount <= 1) return 0;
        float middleIndex = (totalCount - 1) / 2f;
        return (cardIndex - middleIndex) * (2 * maxDeviation) / (totalCount - 1) * 
            (cardIndex < middleIndex? 1: -1); // flip the sign for the second half
    }

    public void LockCards()
    {
        for (int i = 0; i < Hand.Count; i++)
        {
            Hand[i].Playable = false;
        }
    }

    public async Task RoundStart()
    {
        await Hand.ForEach(async card => await card.RoundStart() );
    }

    public async Task RoundEnd()
    {
        await Hand.ForEach(async card => await card.RoundEnd());
    }

    

    public CardModel GetHighestCostCard()
    {
        if (Hand.Count == 0) return null;

        CardModel highestCostCard = Hand[0];
        for (int i = 0; i < Hand.Count; i++)
        {
            if (highestCostCard.CurrentCost < Hand[i].CurrentCost)
            {
                highestCostCard = Hand[i];
            }
        }
        return highestCostCard;
    }

    public CardModel GetLowestCostCard()
    {
        if (Hand.Count == 0) return null;

        CardModel lowestCostCard = Hand[0];
        for (int i = 0; i < Hand.Count; i++)
        {
            if (lowestCostCard.CurrentCost > Hand[i].CurrentCost)
            {
                lowestCostCard = Hand[i];
            }
        }
        return lowestCostCard;
    }

}
