using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EverythingNice : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await ApplyCondition(new Keep(this, 0));
        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        CardModel[] allies = Board.GetUnits(Owner);

        for (int i = 0; i < allies.Length; i++)
        {
            await allies[i].Heal(1);
        }
    }
}
