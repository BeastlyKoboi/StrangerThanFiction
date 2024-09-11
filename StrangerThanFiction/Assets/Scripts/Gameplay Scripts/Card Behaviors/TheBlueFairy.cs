using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheBlueFairy : CardModel
{
    public override uint Id => 8;

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

    private async Task IfCardPlayedIsCostReducedDrawCard(CardPlayState cardPlayState)
    {
        if (cardPlayState.card.BaseCost > cardPlayState.card.CurrentCost)
        {
            await Owner.DrawCard();
        }
    }
}
