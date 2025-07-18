using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TheCoachman : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnRoundStart.AddListener(SummonDonkey);
        return UniTask.CompletedTask;
    }

    protected override async UniTask SummonEffect()
    {
        await SummonDonkey();
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart.RemoveListener(SummonDonkey);
        return UniTask.CompletedTask;
    }

    private async UniTask SummonDonkey()
    {
        CardModel donkey = CardFactory.Instance.CreateCard("Donkey", true, transform, Owner, Board, Title);
        await donkey.Deploy();
        await donkey.Summon();
    }
}
