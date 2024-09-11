using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Candlewick : CardModel
{
    public override uint Id => 1;

    protected override Task SummonEffect()
    {
        Owner.OnUnitSummoned += CandlewickEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
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
