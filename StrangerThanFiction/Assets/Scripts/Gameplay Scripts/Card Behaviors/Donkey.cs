using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Donkey : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnUnitSummoned.AddListener(OnCopySummonGrantMePower);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitSummoned.RemoveListener(OnCopySummonGrantMePower);
        return UniTask.CompletedTask;
    }

    private async UniTask OnCopySummonGrantMePower(CardModel unit)
    {
        if (unit == this)
            return;

        if (unit.Title == Title && unit != this)
        {
            await GrantPower(1);
        }
    }

    private async UniTask OnCopySummonedGrantMePower(CardModel unit)
    {
        if (unit == this) return;

        if (unit.Title == Title && unit != this)
        {
            await GrantPower(unit.CurrentPower);
        }


    }

}
