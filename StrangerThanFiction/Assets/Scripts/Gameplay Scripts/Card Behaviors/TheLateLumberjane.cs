using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheLateLumberjane : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnUnitDestroyed += OnDestroyEffect;
        Owner.enemyPlayer.OnUnitDestroyed += OnDestroyEffect;
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitDestroyed -= OnDestroyEffect;
        Owner.enemyPlayer.OnUnitDestroyed -= OnDestroyEffect;
        return UniTask.CompletedTask;
    }

    private async UniTask OnDestroyEffect(CardModel unit)
    {
        await GrantPower(1);
    }
}
