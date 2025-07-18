using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class MurderOfCrows : CardModel
{
    protected override async UniTask SummonEffect()
    {
        CardModel[] enemies = Board.GetUnits(Owner.enemyPlayer);

        for (int i = 0; i < enemies.Length; i++)
        {
            await enemies[i].TakeDamage(new DamageData(1, this));
        }
    }
}
