using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WittyWhittler : CardModel
{
    public override string Title => "Witty Whittler";
    public override string Description => "When you summon an ally, grant a random enemy Poison 1.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Default.png";

    public override int BaseCost => 3;
    public override int BasePower => 2;
    public override int BasePlotArmor => 4;

    public override async void Start()
    {
        base.Start();

    }

    protected override Task SummonEffect()
    {
        Owner.OnUnitSummoned += OnAllySummonedGrantPoison;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnUnitSummoned -= OnAllySummonedGrantPoison;
        return Task.CompletedTask;
    }

    public async Task OnAllySummonedGrantPoison(CardModel ally)
    {
        CardModel enemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (enemy)
            await enemy.ApplyCondition(Poisoned.StaticName, new Poisoned(enemy, 1));
    }

}
