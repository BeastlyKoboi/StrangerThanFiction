using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class HumptyDumptyPhoenixForm : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnAfterUnitSurvivedDamage.AddListener(OnSurviveDamageGrantPower);
        return base.DeployEffect(deployState);
    }

    protected override async UniTask SummonEffect()
    {
        await ApplyCondition(new Tenacious(this));
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnAfterUnitSurvivedDamage.RemoveListener(OnSurviveDamageGrantPower);
        return base.RemoveEffect(card);
    }

    private async UniTask OnSurviveDamageGrantPower(DamageData damageData)
    {
        if (damageData.target is CardModel unit)
        {
            await unit.GrantPower(1);
        }
    }

}
