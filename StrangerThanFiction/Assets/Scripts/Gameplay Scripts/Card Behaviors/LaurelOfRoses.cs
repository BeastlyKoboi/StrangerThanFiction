using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaurelOfRoses : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyUnitTargets[0])
        {
            await cardPlayState.allyUnitTargets[0].ApplyCondition(new Thorns(cardPlayState.allyUnitTargets[0], 2));
        }
    }
}
