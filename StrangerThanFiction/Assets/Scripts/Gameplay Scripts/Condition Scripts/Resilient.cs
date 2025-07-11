using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Resilient : Condition
{
    public Resilient(CardModel card, int amount) : base(card, amount) { }

    public override UniTask OnAdd()
    {
        card.DamageResistence += amount;
        return UniTask.CompletedTask;
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        if (amount < surplus.amount)
            amount = surplus.amount;
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.DamageResistence -= amount;
        return UniTask.CompletedTask;
    }
}
