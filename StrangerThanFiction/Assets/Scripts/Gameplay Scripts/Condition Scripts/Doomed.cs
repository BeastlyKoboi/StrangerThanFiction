using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Doomed : Condition
{
    public Doomed(CardModel card, int amount) : base(card, amount)
    {

    }

    public override UniTask OnAdd()
    {
        card.Owner.OnRoundEnd.AddListener(OnTrigger);
        return UniTask.CompletedTask;
    }
    public override async UniTask OnTrigger()
    {
        await card.TakeDamage(new DamageData(amount, this));
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        if (amount < surplus.amount)
            amount = surplus.amount;
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.Owner.OnRoundEnd.RemoveListener(OnTrigger);
        return UniTask.CompletedTask;
    }
}
