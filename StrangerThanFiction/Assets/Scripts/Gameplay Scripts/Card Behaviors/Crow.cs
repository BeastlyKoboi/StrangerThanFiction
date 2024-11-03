using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Crow : CardModel
{
    public override uint Id => 13;

    protected override async Task SummonEffect()
    {
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);

        if (randomEnemy)
        { 
            await randomEnemy.TakeDamage(1);
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

        Debug.Log($"Allied crows before merge check: {alliedCrows.Count}");

        if (alliedCrows.Count >= 2)
        {
            await alliedCrows[0].Remove();
            await alliedCrows[1].Remove();

            OnRemove += SummonMurder;
            await Remove();
        }
        
    }

    private async Task SummonMurder(CardModel card)
    {
        CardModel murderOfCrows = CardFactory.Instance.CreateCard("MurderOfCrows", true, transform, Owner, Board, Title);
        await murderOfCrows.Summon();
    }
}
