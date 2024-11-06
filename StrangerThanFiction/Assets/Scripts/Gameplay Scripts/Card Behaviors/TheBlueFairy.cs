using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheBlueFairy : CardModel
{
    protected override Task SummonEffect()
    {
        Owner.OnCardPlayed += IfCardPlayedIsCostReducedDrawCard;
        return Task.CompletedTask;
    }

    protected override Task RemoveEffect(CardModel card)
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
