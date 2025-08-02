using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class TheChosenOne : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnSummon.AddListener(AddConditions);
        OnSurviveDamage.AddListener(CheckIfDestroyed);

        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnSummon.RemoveListener(AddConditions);
        OnSurviveDamage.RemoveListener(CheckIfDestroyed);

        return UniTask.CompletedTask;
    }

    private async UniTask AddConditions()
    {
        await ApplyCondition(new Fated(this, 4));
        await ApplyCondition(new Tenacious(this, 0));
    }

    private async UniTask CheckIfDestroyed(DamageData damageData)
    {
        if (CurrentPlotArmor == 0)
            await this.Destroy();
    }
}
