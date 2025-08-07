using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleRedCap : CardModel
{
    protected async override void Awake()
    {
        base.Awake();

        await ApplyCondition(new Stackable(this, 0));
    }

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnRoundEnd.AddListener(TransformIntoLittleRedCap);
        return UniTask.CompletedTask;
    }

    protected override async UniTask SummonEffect()
    {
        CardModel weakestUnit = Owner.board.GetWeakestUnit(Owner.enemyPlayer);
        if (weakestUnit == null) return;
        await weakestUnit.ApplyCondition(new Poisoned(weakestUnit, 2));
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundEnd.RemoveListener(TransformIntoLittleRedCap);
        return UniTask.CompletedTask;
    }

    private async UniTask TransformIntoLittleRedCap()
    {
        if (CurrentPower >= 5)
        {
            await TransformInto(typeof(BigRedCap).ToString());
        }
    }
}
