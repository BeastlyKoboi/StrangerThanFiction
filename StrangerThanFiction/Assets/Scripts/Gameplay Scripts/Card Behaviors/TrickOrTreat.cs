using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TrickOrTreat : CardModel
{
    protected async override UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyCardTargets.Count != 0)
        {
            await cardPlayState.allyCardTargets[0].GrantPower(3);
        }
        else if (cardPlayState.enemyCardTargets.Count != 0)
        {
            await cardPlayState.enemyCardTargets[0].TakeDamage(new DamageData(3, this));
        }
    }
}
