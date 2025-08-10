using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spice : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        CardModel[] allies = Board.GetUnits(Owner);
        CardModel[] enemies = Board.GetUnits(Owner.enemyPlayer);

        for (int i = 0; i < allies.Length; i++)
        {
            await allies[i].TakeDamage(new DamageData(1, this));
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            await enemies[i].TakeDamage(new DamageData(1, this));
        }

        Owner.CreateCardInHand(typeof(EverythingNice).ToString());
    }
}
