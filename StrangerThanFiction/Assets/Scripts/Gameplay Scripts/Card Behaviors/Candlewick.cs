using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Candlewick : CardModel
{
    public override string Title => "Candlewick";
    public override string Description => "When you summon a unit with a cost of 1 or less, grant it +1 power and +1 plot armor.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/CandleWick.png";

    public override int BaseCost => 2;
    public override int BasePower => 2;
    public override int BasePlotArmor => 1;

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
