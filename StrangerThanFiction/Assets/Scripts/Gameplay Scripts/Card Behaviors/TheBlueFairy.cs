using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheBlueFairy : CardModel
{
    public override string Title => "The Blue Fairy";
    public override string Description => "When you play a card with a cost reduction, draw a card. ";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/The_Blue_Fairy.png";

    public override int BaseCost => 2;
    public override int BasePower => 2;
    public override int BasePlotArmor => 1;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    protected override Task SummonEffect()
    {
        Owner.OnCardPlayed += IfCardPlayedIsCostReducedDrawCard;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnCardPlayed -= IfCardPlayedIsCostReducedDrawCard;
        return Task.CompletedTask;
    }

    private Task IfCardPlayedIsCostReducedDrawCard(CardPlayState cardPlayState)
    {
        if (cardPlayState.card.BaseCost > cardPlayState.card.CurrentCost)
        {
            Owner.DrawCard();
        }
        return Task.CompletedTask;
    }
}
