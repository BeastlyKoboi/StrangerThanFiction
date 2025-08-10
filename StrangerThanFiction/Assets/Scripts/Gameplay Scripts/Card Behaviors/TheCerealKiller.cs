using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TheCerealKiller : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnRoundEnd.AddListener(StrikeWeakestEnemy);
        return base.DeployEffect(deployState);
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundEnd.RemoveListener(StrikeWeakestEnemy);
        return base.RemoveEffect(card);
    }

    private async UniTask StrikeWeakestEnemy()
    {
        CardModel weakestEnemy = Board.GetWeakestUnit(Owner.enemyPlayer);

        if (!weakestEnemy) return;
        
        await Strike(weakestEnemy);

        if (!weakestEnemy || weakestEnemy.IsRemoved) return;

        await weakestEnemy.Strike(this);
    }
}
