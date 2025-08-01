using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class WittyWhittler : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnBeforeUnitSummoned.AddListener(OnAllySummonedGrantPoison);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnBeforeUnitSummoned.RemoveListener(OnAllySummonedGrantPoison);
        return UniTask.CompletedTask;
    }

    public async UniTask OnAllySummonedGrantPoison(CardModel ally)
    {
        if (ally == this)
            return;

        CardModel enemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (enemy)
            await enemy.ApplyCondition(new Poisoned(enemy, 1));
    }

}
