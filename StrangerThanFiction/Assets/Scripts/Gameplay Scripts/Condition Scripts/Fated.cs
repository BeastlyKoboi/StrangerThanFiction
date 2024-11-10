using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Fated : Condition
{
    public override uint Id => 2;
    public Fated(CardModel card, int amount) : base(card, amount) { }

    public override UniTask OnAdd()
    {
        card.Owner.OnRoundEnd.AddListener(OnTrigger);
        return UniTask.CompletedTask;
    }
    public override async UniTask OnTrigger()
    {
        await card.GrantPower(amount);
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
