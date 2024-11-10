using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheCoachman : CardModel
{
    protected override async UniTask SummonEffect()
    {
        await SummonDonkey();
        Owner.OnRoundStart += SummonDonkey;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart -= SummonDonkey;
        return UniTask.CompletedTask;
    }

    private async UniTask SummonDonkey()
    {
        CardModel donkey = CardFactory.Instance.CreateCard("Donkey", true, transform, Owner, Board, Title);

        await donkey.Summon();
    }
}
