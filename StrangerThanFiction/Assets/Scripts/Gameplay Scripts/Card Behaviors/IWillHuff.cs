using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class IWillHuff : CardModel
{
    public override async void Start()
    {
        base.Start();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(1);

        Owner.CreateCardInDeck("AndIWillPuff");
        Owner.CreateCardInDeck("AndIWillPuff");
    }
}
