using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class Donkey : CardModel
{
    public override string Title => "Donkey";
    public override string Description => "Whenever you summon another copy of me, grant me +1 power.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Donkey.png";

    public override int BaseCost => 1;
    public override int BasePower => 1;
    public override int BasePlotArmor => 1;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

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
