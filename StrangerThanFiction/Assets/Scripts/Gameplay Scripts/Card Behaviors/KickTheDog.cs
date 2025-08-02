using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickTheDog : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyUnitTargets == null || cardPlayState.allyUnitTargets.Count == 0)
            return;

        CardModel ally = cardPlayState.allyUnitTargets[0];
        await ally.TakeDamage(new DamageData(3, this));

        if (ally.IsRemoved)
        {
            await Owner.DrawCard();
            await Owner.DrawCard();
        }
        else
        {
            Owner.OnRoundStart.AddListener(DrawCardsAtNextRoundStart);
        }

    }

    private async UniTask DrawCardsAtNextRoundStart()
    {
        await Owner.DrawCard();
        await Owner.DrawCard();
        Owner.OnRoundStart.RemoveListener(DrawCardsAtNextRoundStart);
    }
}
