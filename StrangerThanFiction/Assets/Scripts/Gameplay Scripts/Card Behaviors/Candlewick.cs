using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Candlewick : CardModel
{
    protected override Task SummonEffect()
    {
        Owner.OnUnitSummoned += CandlewickEffect;
        return Task.CompletedTask;
    }

    protected override Task RemoveEffect(CardModel card)
    {
        Owner.OnUnitSummoned -= CandlewickEffect;
        return Task.CompletedTask;
    }

    private async Task CandlewickEffect(CardModel unit)
    {
        if (unit.CurrentCost <= 1)
        {
            await unit.GrantPower(1);
            await unit.GrantPlotArmor(1);
        }
    }   
}
