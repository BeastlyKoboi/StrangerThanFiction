using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public sealed class SaveTheCat : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        if (cardPlayState.allyUnitTargets == null || cardPlayState.allyUnitTargets.Count == 0) 
            return; 

        CardModel ally = cardPlayState.allyUnitTargets[0];
        await ally.Heal(3);
        await ally.ApplyCondition(new Tenacious(ally));

        if (ally.CurrentPower == ally.MaxPower)
        {
            await Owner.DrawCard();
        } 
    }
}
