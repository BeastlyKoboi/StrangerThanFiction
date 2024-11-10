using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WittyWhittler : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnUnitSummoned.AddListener(OnAllySummonedGrantPoison);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitSummoned.RemoveListener(OnAllySummonedGrantPoison);
        return UniTask.CompletedTask;
    }

    public async UniTask OnAllySummonedGrantPoison(CardModel ally)
    {
        CardModel enemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (enemy)
            await enemy.ApplyCondition(new Poisoned(enemy, 1));
    }

}
