using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Candlewick : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnBeforeUnitSummoned.AddListener(CandlewickEffect);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnBeforeUnitSummoned.RemoveListener(CandlewickEffect);
        return UniTask.CompletedTask;
    }

    private async UniTask CandlewickEffect(CardModel unit)
    {
        if (unit == this)
            return;

        if (unit.CurrentCost <= 1)
        {
            await unit.GrantPower(1);
            await unit.GrantPlotArmor(1);
        }
    }   
}
