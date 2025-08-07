using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ImpulsiveTinkering : Boon
{
    private Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;

        // At round start, summon a Magical Woodcarving,
        player.OnRoundStart.AddListener(SummonMagicalWoodcarving);

        return UniTask.CompletedTask;
    }

    public override UniTask ExitCombat()
    {
        player.OnRoundStart.RemoveListener(SummonMagicalWoodcarving);

        return UniTask.CompletedTask;
    }

    private async UniTask SummonMagicalWoodcarving(RoundStartState roundStartState)
    {
        CardModel woodcarving = CardFactory.Instance.CreateCard("MagicalWoodcarving", true, player.transform, player, player.board, "Impulsive Tinkering");
        await woodcarving.Deploy();
        await woodcarving.Summon();
    }
}
