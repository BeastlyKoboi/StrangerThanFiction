using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DonkeyFever : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.enemyUnitTargets.Count > 0 && cardPlayState.enemyUnitTargets != null)
        {
            await cardPlayState.enemyUnitTargets[0].TransformInto("Donkey");
        }
    }
}
