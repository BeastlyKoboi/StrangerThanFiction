using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AndIWillPuff : CardModel
{
    public override async void Start()
    {
        base.Start();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(2);
        await cardPlayState.enemyUnitTargets[1].TakeDamage(2);

        Owner.CreateCardInDeck("AndIWillBlowYouAway");
        Owner.CreateCardInDeck("AndIWillBlowYouAway");
    }
}
