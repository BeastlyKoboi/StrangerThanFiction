using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Donkey : CardModel
{
    protected async override void Awake()
    {
        base.Awake();

        await ApplyCondition(new Stackable(this, 0));
    }

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnRoundEnd.AddListener(TransformIntoRascal);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundEnd.RemoveListener(TransformIntoRascal);
        return UniTask.CompletedTask;
    }

    protected async UniTask TransformIntoRascal()
    {
        if (CurrentPower >= 5)
        {
            await TransformInto("Rascals");
        }
    }




}
