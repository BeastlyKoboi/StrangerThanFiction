using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HealingBroth : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyUnitTargets[0])
        {
            await cardPlayState.allyUnitTargets[0].ApplyCondition(new Cured(cardPlayState.allyUnitTargets[0], 3));
        }
    }
}
