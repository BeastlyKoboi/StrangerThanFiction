using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DramaticDuel : CardModel
{

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyUnitTargets.Count > 0 && cardPlayState.enemyUnitTargets.Count > 0)
        {
            await SimultaneousStrike(cardPlayState.allyUnitTargets[0], cardPlayState.enemyUnitTargets[0]);
        }
    }

}
