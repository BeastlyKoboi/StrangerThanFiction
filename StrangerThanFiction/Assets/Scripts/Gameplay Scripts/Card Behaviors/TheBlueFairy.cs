using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheBlueFairy : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnCardPlayed += IfCardPlayedIsCostReducedDrawCard;
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnCardPlayed -= IfCardPlayedIsCostReducedDrawCard;
        return UniTask.CompletedTask;
    }

    private async UniTask IfCardPlayedIsCostReducedDrawCard(CardPlayState cardPlayState)
    {
        if (cardPlayState.card.BaseCost > cardPlayState.card.CurrentCost)
        {
            await Owner.DrawCard();
        }
    }
}
