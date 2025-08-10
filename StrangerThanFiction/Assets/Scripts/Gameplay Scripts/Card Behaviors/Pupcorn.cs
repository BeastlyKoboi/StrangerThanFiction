using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Pupcorn : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await this.ApplyCondition(new Stackable(this, 0));
    }

    protected override async UniTask SummonEffect()
    {
        CardModel[] allies = Board.GetUnits(Owner);

        for (int i = 0; i < allies.Length; i++)
        {
            await allies[i].Heal(1);
        }
    }

}
