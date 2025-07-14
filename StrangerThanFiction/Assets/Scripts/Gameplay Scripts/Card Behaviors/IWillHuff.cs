using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class IWillHuff : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(new DamageData(1, this));

        Owner.CreateCardInDeck("AndIWillPuff");
        Owner.CreateCardInDeck("AndIWillPuff");
    }
}
