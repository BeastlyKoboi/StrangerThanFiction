using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatedArtisan : CardModel
{
    public override string Title => "Animated Artisan";
    public override string Description => "On Round Start, create a Magical Woodcarving in deck.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Default.png";

    public override int BaseCost => 3;
    public override int BasePower => 2;
    public override int BasePlotArmor => 4;

    protected override Task SummonEffect()
    {
        Owner.OnRoundStart += RoundStartEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnRoundStart -= RoundStartEffect;
        return Task.CompletedTask;
    }

    private Task RoundStartEffect()
    {
        Owner.CreateCardInDeck(typeof(MagicalWoodcarving).ToString());
        return Task.CompletedTask;
    }
}
