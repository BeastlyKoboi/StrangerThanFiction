using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Helpless : Condition
{
    public Helpless(CardModel card, int amount) : base(card, amount)
    {
    }

    public override UniTask OnAdd()
    {
        card.DamageDealtMultiplier *= 2f;
        card.Owner.OnRoundEnd.AddListener(OnTrigger);
        return UniTask.CompletedTask;
    }
    public override async UniTask OnTrigger()
    {
        amount -= 1;
        if (amount == 0)
        {
            await card.RemoveCondition(Name);
        }
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        if (surplus is Helpless helplessSurplus)
        {
            amount += helplessSurplus.amount;
        }
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.DamageDealtMultiplier /= 2f;
        card.Owner.OnRoundEnd.RemoveListener(OnTrigger);
        return UniTask.CompletedTask;
    }
}
