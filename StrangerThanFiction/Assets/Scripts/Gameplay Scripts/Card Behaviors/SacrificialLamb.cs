using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SacrificialLamb : CardModel
{
    protected override Task SummonEffect()
    {
        Owner.OnUnitSummoned += Sacrifice;
        return Task.CompletedTask;
    }

    protected override Task RemoveEffect(CardModel card)
    {
        Owner.OnUnitSummoned -= Sacrifice;
        return base.RemoveEffect(card);
    }

    private async Task Sacrifice(CardModel unit)
    {
        await unit.Strike(this);
        await unit.GrantPower(CurrentPower);
    }
}
