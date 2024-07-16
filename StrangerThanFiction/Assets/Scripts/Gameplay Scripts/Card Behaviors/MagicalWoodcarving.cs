using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MagicalWoodcarving : CardModel
{
    public override string Title => "Magical Woodcarving";
    public override string Description => "At round end, I strike a random enemy, then I am destroyed.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Magical_Woodcarving.png";

    public override int BaseCost => 1;
    public override int BasePower => 7;
    public override int BasePlotArmor => 0;

    public override void Start() => base.Start();
    protected override Task SummonEffect()
    {
        OnRoundEnd += RoundEndEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        OnRoundEnd -= RoundEndEffect;
        return Task.CompletedTask;
    }

    protected async Task RoundEndEffect()
    {
        Debug.Log($"MWC Round End Effect Called By: {Owner.gameObject.name}");
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (randomEnemy)
        {
            await randomEnemy.TakeDamage(CurrentPower);
        }
    }
}
