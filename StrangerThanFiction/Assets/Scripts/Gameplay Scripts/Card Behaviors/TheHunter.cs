using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TheHunter : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.allyUnitTargets[0].Strike(cardPlayState.enemyUnitTargets[0]);

        await cardPlayState.allyUnitTargets[0].Destroy();
    }
}
