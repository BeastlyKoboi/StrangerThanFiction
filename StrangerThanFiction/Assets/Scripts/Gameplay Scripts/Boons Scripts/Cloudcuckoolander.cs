using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Cloudcuckoolander : Boon
{
    private Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        player.OnRoundStart.AddListener(CreateTallTaleInHand);
        return UniTask.CompletedTask;
    }

    public override UniTask ExitCombat()
    {
        player.OnRoundStart.RemoveListener(CreateTallTaleInHand);
        return UniTask.CompletedTask;
    }

    private UniTask CreateTallTaleInHand()
    {
        player.CreateCardInHand(typeof(TallTale).ToString());
        return UniTask.CompletedTask;
    }
}
