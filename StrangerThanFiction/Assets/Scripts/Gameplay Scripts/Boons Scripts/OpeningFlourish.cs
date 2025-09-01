using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningFlourish : Boon
{
    private Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        player.OnAfterCardPlayed.AddListener(GainInk);
        return UniTask.CompletedTask;
    }

    private UniTask GainInk(CardPlayState cardPlayState)
    {
        if (player.NumCardsPlayedThisRound != 0) return UniTask.CompletedTask;
        player.CurrentMana += cardPlayState.card.CurrentCost;
        return UniTask.CompletedTask;
    }
}
