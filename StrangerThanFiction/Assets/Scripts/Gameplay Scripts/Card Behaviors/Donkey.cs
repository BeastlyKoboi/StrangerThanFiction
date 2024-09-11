using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Donkey : CardModel
{
    public override uint Id => 2;

    protected override Task SummonEffect()
    {
        Owner.OnUnitSummoned += OnCopySummonGrantMePower;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnUnitSummoned -= OnCopySummonGrantMePower;
        return Task.CompletedTask;
    }

    private async Task OnCopySummonGrantMePower(CardModel unit)
    {
        if (unit.Title == Title && unit != this)
        {
            await GrantPower(1);
        }
    }
}
