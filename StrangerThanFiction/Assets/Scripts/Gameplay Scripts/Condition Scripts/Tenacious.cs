using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tenacious : Condition
{
    public Tenacious(CardModel card, int amount = 0) : base(card, amount) { }

    public override UniTask OnAdd()
    {
        card.OnRoundEnd.AddListener(OnTrigger);
        return UniTask.CompletedTask;
    }

    public override async UniTask OnTrigger()
    {
        if (card.CurrentPower < card.MaxPower)
            await card.Heal(card.MaxPower - card.CurrentPower);
    }

    public override UniTask OnSurplus(Condition surplus)
    {
        return UniTask.CompletedTask;
    }

    public override UniTask OnRemove()
    {
        card.OnRoundEnd.RemoveListener(OnTrigger);
        return UniTask.CompletedTask;
    }
}
