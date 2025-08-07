using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRed : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnRoundStart.AddListener(CreateBigRedCapInHand);
        Owner.enemyPlayer.OnAfterUnitConditionApplied.AddListener(GrantSelfPower);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart.RemoveListener(CreateBigRedCapInHand);
        Owner.enemyPlayer.OnAfterUnitConditionApplied.RemoveListener(GrantSelfPower);
        return UniTask.CompletedTask;
    }

    private UniTask CreateBigRedCapInHand(RoundStartState roundStartState)
    {
        Owner.CreateCardInHand(typeof(BigRedCap).ToString());
        return UniTask.CompletedTask;
    }

    private async UniTask GrantSelfPower(Condition condition)
    {
        if (condition.ConditionType == ConditionType.Negative)
        {
            await GrantPower(1);
        }
    }
}
