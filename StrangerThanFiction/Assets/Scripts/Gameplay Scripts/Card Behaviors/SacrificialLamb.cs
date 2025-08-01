using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class SacrificialLamb : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnBeforeUnitSummoned.AddListener(Sacrifice);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnBeforeUnitSummoned.RemoveListener(Sacrifice);
        return base.RemoveEffect(card);
    }

    private async UniTask Sacrifice(CardModel unit)
    {
        if (unit == this)
            return;

        await unit.Strike(this);
        await unit.GrantPower(CurrentPower);
    }
}
