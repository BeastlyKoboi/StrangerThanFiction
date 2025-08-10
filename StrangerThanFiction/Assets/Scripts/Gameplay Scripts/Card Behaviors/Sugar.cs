using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sugar : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyUnitTargets[0])
        {
            await cardPlayState.allyUnitTargets[0].ApplyCondition(new Cured(cardPlayState.allyUnitTargets[0], 3));
        }

        await SimultaneousStrike(cardPlayState.allyUnitTargets[0], cardPlayState.enemyUnitTargets[0]);

        Owner.CreateCardInHand(typeof(Spice).ToString());
    }
}
