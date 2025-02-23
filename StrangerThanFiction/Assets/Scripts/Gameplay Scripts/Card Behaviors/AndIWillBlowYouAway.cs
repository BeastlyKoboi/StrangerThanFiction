using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AndIWillBlowYouAway : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(new DamageData(3, this));
        await cardPlayState.enemyUnitTargets[1].TakeDamage(new DamageData(3, this));
        await cardPlayState.enemyUnitTargets[2].TakeDamage(new DamageData(3, this));
    }
}
