using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuiltDifferent : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        CardModel target = cardPlayState.allyUnitTargets[0];

        if (target)
        {
            await target.ApplyCondition(new Fated(target, 2));
        }
    }
}
