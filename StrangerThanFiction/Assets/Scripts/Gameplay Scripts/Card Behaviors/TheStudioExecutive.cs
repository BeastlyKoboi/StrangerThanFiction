using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public sealed class TheStudioExecutive : CardModel
{
    protected override async UniTask SummonEffect()
    {
        CardModel cardToDiscard = null;
        List<CardModel> cardsInHand = new List<CardModel>(); 
        cardsInHand.AddRange(Owner.handManager.Hand.cards);
        cardsInHand.Remove(this);

        if (cardsInHand.Count > 0)
        {
            cardToDiscard = cardsInHand[UnityEngine.Random.Range(0, cardsInHand.Count)];

            await Owner.DiscardCard(cardToDiscard);
        }

        List<string> cardNames = new List<string>();
        List<CardModel> cardModels = new List<CardModel>();

        foreach (CardModel unit in Board.GetUnits(Owner))
        {
            if (!cardNames.Contains(unit.Title))
                cardNames.Add(unit.Title);
        }
        
        Owner.Deck.ForEach(card => {
            if (cardNames.Contains(card.Title))
                cardModels.Add(card);
        });

        for (int i = 0; i < 2; i++)
        {
            if (cardModels.Count > 0)
            {
                CardModel cardToDraw = cardModels[Random.Range(0, cardModels.Count)];
                cardModels.Remove(cardToDraw);
                await Owner.DrawCard(cardToDraw);
            }
        }

    }
}
