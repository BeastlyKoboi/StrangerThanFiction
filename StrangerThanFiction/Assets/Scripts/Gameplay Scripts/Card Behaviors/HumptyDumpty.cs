using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class HumptyDumpty : CardModel
{
    private int powerSurvived = 0;

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnSurviveDamage.AddListener(OnSurviveDamageGrantPower);
        return base.DeployEffect(deployState);
    }

    protected override async UniTask SummonEffect()
    {
        await ApplyCondition(new Tenacious(this));
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnSurviveDamage.RemoveListener(OnSurviveDamageGrantPower);
        return base.RemoveEffect(card);
    }

    private async UniTask OnSurviveDamageGrantPower(DamageData damageData)
    {
        powerSurvived += damageData.damage;

        await GrantPower(1);

        if (powerSurvived >= 5)
        {
            await TransformInto(typeof(HumptyDumptyPhoenixForm).ToString());
        }
    }
}
