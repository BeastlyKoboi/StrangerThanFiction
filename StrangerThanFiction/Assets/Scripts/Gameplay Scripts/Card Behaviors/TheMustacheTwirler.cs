using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TheMustacheTwirler : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnAfterUnitSurvivedDamage.AddListener(ApplyResilient);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnAfterUnitSurvivedDamage.RemoveListener(ApplyResilient);
        return UniTask.CompletedTask;
    }

    private async UniTask ApplyResilient(DamageData damageData)
    {
        if (damageData.target is CardModel unit)
        {
            await unit.ApplyCondition(new Resilient(unit, 1));
        }
    }
}
