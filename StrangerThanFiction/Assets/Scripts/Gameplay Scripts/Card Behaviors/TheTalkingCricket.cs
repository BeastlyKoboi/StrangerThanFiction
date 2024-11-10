using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheTalkingCricket : CardModel
{
    protected override async UniTask SummonEffect()
    {
        CardModel highestCostCard = Owner.handManager.GetHighestCostCard();
        if (highestCostCard)
        {
            await highestCostCard.GrantCostModification(-1);
        }
    }

    protected override UniTask DestroyEffect(CardModel card)
    {
        Owner.CreateCardInDeck(typeof(TheTalkingCricket).ToString());
        return UniTask.CompletedTask;
    }

}
