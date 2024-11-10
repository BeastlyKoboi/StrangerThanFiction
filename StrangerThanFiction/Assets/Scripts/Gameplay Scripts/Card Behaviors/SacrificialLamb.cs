using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SacrificialLamb : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnUnitSummoned.AddListener(Sacrifice);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitSummoned.RemoveListener(Sacrifice);
        return base.RemoveEffect(card);
    }

    private async UniTask Sacrifice(CardModel unit)
    {
        await unit.Strike(this);
        await unit.GrantPower(CurrentPower);
    }
}
