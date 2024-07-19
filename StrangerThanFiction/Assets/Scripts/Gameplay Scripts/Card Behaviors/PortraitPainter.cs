using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PortraitPainter : CardModel
{
    public override string Title => "Portrait Painter";
    public override string Description => "When you draw a unit, grant it +2 Power.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Default.png";

    public override int BaseCost => 2;
    public override int BasePower => 1;
    public override int BasePlotArmor => 4;

    protected override Task SummonEffect()
    {
        Owner.OnCardDrawn += OnDrawEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnCardDrawn -= OnDrawEffect;
        return Task.CompletedTask;
    }

    private async Task OnDrawEffect(CardModel card)
    {
        if (card.Type == CardType.Unit)
        {
            await card.GrantPower(2);
        }
    }

}
