using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheTalkingCricket : CardModel
{
    public override uint Id => 10;

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
