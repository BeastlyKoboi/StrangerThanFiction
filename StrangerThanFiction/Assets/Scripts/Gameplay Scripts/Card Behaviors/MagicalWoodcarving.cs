using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class MagicalWoodcarving : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        OnRoundEnd.AddListener(RoundEndEffect);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundEnd.RemoveListener(RoundEndEffect);
        return UniTask.CompletedTask;
    }

    private async UniTask RoundEndEffect()
    {
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (randomEnemy)
        {
            await Strike(randomEnemy);
        }

        await Destroy();
    }
}
