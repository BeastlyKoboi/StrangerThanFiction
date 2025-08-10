using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProperDosage : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        CardModel[] units = Board.GetUnits(Owner.enemyPlayer);

        int healAmount = 0;

        foreach (CardModel unit in units)
        {
            if (unit.HasCondition(typeof(Poisoned).ToString()))
            {
                Poisoned poisoned = (Poisoned)unit.GetConditions().FirstOrDefault((Condition condition) => condition is Poisoned);
                if (healAmount < poisoned.amount) 
                    healAmount = poisoned.amount;
            }
        }

        await cardPlayState.allyUnitTargets[0].Heal(healAmount);
    }
}
