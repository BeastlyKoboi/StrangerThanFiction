using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cured : Condition
{
    public Cured(CardModel card, int amount) : base(card, amount)
    {
    }
    public override UniTask OnAdd()
    {
        card.Owner.OnRoundEnd.AddListener(OnTrigger);
        return UniTask.CompletedTask;
    }
    public override async UniTask OnTrigger()
    {
        await card.Heal(amount);
        amount -= 1;
        if (amount == 0)
        {
            await card.RemoveCondition(Name);
        }
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        if (surplus is Cured curedSurplus)
        {
            amount += curedSurplus.amount;
        }
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.Owner.OnRoundEnd.RemoveListener(OnTrigger);
        return UniTask.CompletedTask;
    }
}
