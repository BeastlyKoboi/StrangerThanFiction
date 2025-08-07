using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Crow : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await this.ApplyCondition(new Stackable(this, 0));
    }

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnRoundEnd.AddListener(TransformIntoMurderOfCrows);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundEnd.RemoveListener(TransformIntoMurderOfCrows);
        return UniTask.CompletedTask;
    }

    protected override async UniTask SummonEffect()
    {
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);

        if (randomEnemy)
        {
            await randomEnemy.TakeDamage(new DamageData(1, this));
        }
    }

    private async UniTask TransformIntoMurderOfCrows()
    {
        if (CurrentPower >= 5)
        {
            await TransformInto(typeof(MurderOfCrows).ToString());
        }
    }

}
