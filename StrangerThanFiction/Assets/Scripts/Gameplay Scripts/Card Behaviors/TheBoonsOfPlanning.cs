using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class TheBoonsOfPlanning : CardModel
{
    protected override Task PlayEffect(CardPlayState cardPlayState)
    {
        cardPlayState.card.Owner.OnCardPlayed += CreateCopyOfNextCardPlayed;
        return Task.CompletedTask;
    }

    private static async Task CreateCopyOfNextCardPlayed(CardPlayState cardPlayState)
    {
        cardPlayState.card.Owner.CreateCardInDiscard(cardPlayState.card.GetType().Name);
        cardPlayState.card.Owner.OnCardPlayed -= CreateCopyOfNextCardPlayed;
    }

}
