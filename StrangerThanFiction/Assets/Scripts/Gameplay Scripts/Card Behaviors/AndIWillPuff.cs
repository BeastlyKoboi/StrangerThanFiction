using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class AndIWillPuff : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(new DamageData(2, this));
        await cardPlayState.enemyUnitTargets[1].TakeDamage(new DamageData(2, this));

        Owner.CreateCardInDeck(typeof(AndIWillBlowYouAway).ToString());
        Owner.CreateCardInDeck(typeof(AndIWillBlowYouAway).ToString());
    }
}
