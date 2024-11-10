using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class TheBoonsOfPlanning : CardModel
{
    protected override UniTask PlayEffect(CardPlayState cardPlayState)
    {
        cardPlayState.card.Owner.OnCardPlayed.AddListener(CreateCopyOfNextCardPlayed);
        return UniTask.CompletedTask;
    }

    private static async UniTask CreateCopyOfNextCardPlayed(CardPlayState cardPlayState)
    {
        cardPlayState.card.Owner.CreateCardInDiscard(cardPlayState.card.GetType().Name);
        cardPlayState.card.Owner.OnCardPlayed.RemoveListener(CreateCopyOfNextCardPlayed);
    }

}
