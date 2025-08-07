using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveSlap : Boon
{
    private Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        player.OnRoundStart.AddListener(CreateDramaticDuelInHand);
        return UniTask.CompletedTask;
    }

    public override UniTask ExitCombat()
    {
        player.OnRoundStart.RemoveListener(CreateDramaticDuelInHand);
        return UniTask.CompletedTask;
    }

    private UniTask CreateDramaticDuelInHand(RoundStartState roundStartState)
    {
        player.CreateCardInHand(typeof(DramaticDuel).ToString());
        return UniTask.CompletedTask;
    }
}
