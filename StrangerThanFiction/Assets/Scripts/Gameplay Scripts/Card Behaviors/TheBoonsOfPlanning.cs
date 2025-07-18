using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public sealed class TheBoonsOfPlanning : CardModel
{
    protected override UniTask PlayEffect(CardPlayState cardPlayState)
    {
        cardPlayState.card.Owner.OnAfterCardPlayed.AddListener(CreateCopyOfNextCardPlayed);
        return UniTask.CompletedTask;
    }

    private static UniTask CreateCopyOfNextCardPlayed(CardPlayState cardPlayState)
    {
        cardPlayState.card.Owner.CreateCardInDiscard(cardPlayState.card.GetType().Name);
        cardPlayState.card.Owner.OnAfterCardPlayed.RemoveListener(CreateCopyOfNextCardPlayed);
        return UniTask.CompletedTask;
    }

}
