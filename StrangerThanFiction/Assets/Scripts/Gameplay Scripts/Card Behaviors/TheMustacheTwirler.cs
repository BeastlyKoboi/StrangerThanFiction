using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TheMustacheTwirler : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnRoundStart.AddListener(ApplyHelpless);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart.RemoveListener(ApplyHelpless);
        return UniTask.CompletedTask;
    }

    private async UniTask ApplyHelpless(RoundStartState roundStartState)
    {
        CardModel unit = Board.GetWeakestUnit(Owner.enemyPlayer);

        if (unit)
        {
            await unit.ApplyCondition(new Helpless(unit, 1));
        }
    }
}
