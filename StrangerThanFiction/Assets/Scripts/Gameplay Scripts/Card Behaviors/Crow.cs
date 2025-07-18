using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Crow : CardModel
{
    protected override async UniTask SummonEffect()
    {
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);

        if (randomEnemy)
        { 
            await randomEnemy.TakeDamage(new DamageData(1, this));
        }

        // make checks for murder of crows

        CardModel[] allies = Board.GetUnits(Owner);
        List<CardModel> alliedCrows = new List<CardModel>();

        foreach (CardModel card in allies)
        {
            if (card.name == name && card != this)
            {
                alliedCrows.Add(card);
            }
        }

        if (alliedCrows.Count >= 2)
        {
            await alliedCrows[0].Remove();
            await alliedCrows[1].Remove();

            OnRemove.AddListener(SummonMurder);
            await Remove();
        }
        
    }

    private async UniTask SummonMurder(CardModel card)
    {
        CardModel murderOfCrows = CardFactory.Instance.CreateCard("MurderOfCrows", true, transform, Owner, Board, Title);
        await murderOfCrows.Deploy();
        await murderOfCrows.Summon();
    }
}
