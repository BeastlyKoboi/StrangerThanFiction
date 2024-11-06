using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BuiltDifferent : CardModel
{
    protected override async Task PlayEffect(CardPlayState cardPlayState)
    {
        CardModel target = cardPlayState.allyUnitTargets[0];

        if (target)
        {
            await target.ApplyCondition(new Fated(target, 2));
        }
    }
}
