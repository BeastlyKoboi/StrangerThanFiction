using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Donkey : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnUnitSummoned += OnCopySummonGrantMePower;
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitSummoned -= OnCopySummonGrantMePower;
        return UniTask.CompletedTask;
    }

    private async UniTask OnCopySummonGrantMePower(CardModel unit)
    {
        if (unit.Title == Title && unit != this)
        {
            await GrantPower(1);
        }
    }
}
