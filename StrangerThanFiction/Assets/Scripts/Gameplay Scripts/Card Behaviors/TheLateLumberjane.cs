using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheLateLumberjane : CardModel
{
    public override string Title => "The Late Lumberjane";
    public override string Description => "When a unit dies, grant me +2 power.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Default.png";

    public override int BaseCost => 2;
    public override int BasePower => 1;
    public override int BasePlotArmor => 7;

    protected override Task SummonEffect()
    {
        Owner.OnUnitDestroyed += OnDestroyEffect;
        Owner.enemyPlayer.OnUnitDestroyed += OnDestroyEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnUnitDestroyed -= OnDestroyEffect;
        Owner.enemyPlayer.OnUnitDestroyed -= OnDestroyEffect;
        return Task.CompletedTask;
    }

    private async Task OnDestroyEffect(CardModel unit)
    {
        await GrantPower(2);
    }
}
