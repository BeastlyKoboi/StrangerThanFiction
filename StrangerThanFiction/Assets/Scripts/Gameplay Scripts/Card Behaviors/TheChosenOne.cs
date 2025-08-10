using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class TheChosenOne : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnSurviveDamage.AddListener(CheckIfDestroyed);

        return UniTask.CompletedTask;
    }

    protected override async UniTask SummonEffect()
    {
        await ApplyCondition(new Fated(this, 4));
        await ApplyCondition(new Tenacious(this, 0));
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnSurviveDamage.RemoveListener(CheckIfDestroyed);

        return UniTask.CompletedTask;
    }

    private async UniTask CheckIfDestroyed(DamageData damageData)
    {
        if (CurrentPlotArmor == 0)
            await this.Destroy();
    }
}
