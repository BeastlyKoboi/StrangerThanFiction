using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Payoff : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        foreach (CardModel unit in Board.GetUnits(Owner))
            await unit.TakeDamage(new DamageData(3, this));

        foreach (CardModel unit in Board.GetUnits(Owner.enemyPlayer))
            await unit.TakeDamage(new DamageData(3, this));

        if (Owner.NumUnitsHealedThisCombat > 0 || Owner.NumUnitsRevivedThisCombat > 0)
        {
            foreach (CardModel unit in Board.GetUnits(Owner.enemyPlayer))
                await unit.TakeDamage(new DamageData(3, this));
        }

    }
}
