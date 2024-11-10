using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class GrowthSpurt : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        CardModel weakestUnit = cardPlayState.allyUnitTargets[0];// Board.GetWeakestUnit(Owner);

        if (weakestUnit)
        {
           await weakestUnit.GrantPower(4);
        }
    }

}
