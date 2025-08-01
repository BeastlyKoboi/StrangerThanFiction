using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodRoleModel : CardModel
{
    protected override UniTask PlayEffect(CardPlayState cardPlayState)
    {
        Owner.CreateCardInHand(cardPlayState.allyUnitTargets[0].GetType().Name);
        return UniTask.CompletedTask;
    }
}
