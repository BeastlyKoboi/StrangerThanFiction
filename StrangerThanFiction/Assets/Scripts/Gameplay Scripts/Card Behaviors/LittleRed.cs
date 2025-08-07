using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleRed : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnRoundStart.AddListener(CreateLittleRedCapInHand);
        Owner.enemyPlayer.OnAfterUnitConditionApplied.AddListener(GrantSelfPower);
        OnGrantPower.AddListener(TransformIntoBigRed);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart.RemoveListener(CreateLittleRedCapInHand);
        Owner.enemyPlayer.OnAfterUnitConditionApplied.RemoveListener(GrantSelfPower);
        OnGrantPower.RemoveListener(TransformIntoBigRed);
        return UniTask.CompletedTask;
    }

    private UniTask CreateLittleRedCapInHand(RoundStartState roundStartState)
    {
        Owner.CreateCardInHand(typeof(LittleRedCap).ToString());
        return UniTask.CompletedTask;
    }

    private async UniTask GrantSelfPower(Condition condition)
    {
        if (condition.ConditionType == ConditionType.Negative)
        {
            await GrantPower(1);
        }
    }

    private async UniTask TransformIntoBigRed(int powerGranted)
    {
        if (CurrentPower >= 10)
        {
            await TransformInto(typeof(BigRed).ToString());
        }
    }

}
