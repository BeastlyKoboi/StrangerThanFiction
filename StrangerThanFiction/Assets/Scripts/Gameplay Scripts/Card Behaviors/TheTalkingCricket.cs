using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheTalkingCricket : CardModel
{
    public override string Title => "The Talking Cricket";
    public override string Description => "On summon, grant the highest cost card in hand -1 cost. On destroy, create The Talking Cricket in deck.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/The_Talking_Cricket.png";

    public override int BaseCost => 1;
    public override int BasePower => 2;
    public override int BasePlotArmor => 0;

    protected override async Task SummonEffect()
    {
        CardModel highestCostCard = Owner.handManager.GetHighestCostCard();
        if (highestCostCard)
        {
            await highestCostCard.GrantCostModification(-1);
        }
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.CreateCardInDeck(typeof(TheTalkingCricket).ToString());
        return Task.CompletedTask;
    }

}
