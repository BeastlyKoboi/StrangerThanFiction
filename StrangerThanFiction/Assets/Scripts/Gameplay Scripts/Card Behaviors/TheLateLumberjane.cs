using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheLateLumberjane : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnUnitDestroyed.AddListener(OnDestroyEffect);
        Owner.enemyPlayer.OnUnitDestroyed.AddListener(OnDestroyEffect);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitDestroyed.RemoveListener(OnDestroyEffect);
        Owner.enemyPlayer.OnUnitDestroyed.RemoveListener(OnDestroyEffect);
        return UniTask.CompletedTask;
    }

    private async UniTask OnDestroyEffect(CardModel unit)
    {
        await GrantPower(1);
    }
}
