using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Guarded : Condition
{
    public override uint Id => 3;
    public Guarded(CardModel card, int amount) : base(card, amount) { }

    public override UniTask OnAdd()
    {
        card.Owner.OnRoundEnd += OnTrigger;
        return UniTask.CompletedTask;
    }
    public override async UniTask OnTrigger()
    {
        if (card.CurrentPlotArmor < amount)
            await card.GrantPlotArmor(amount - card.CurrentPlotArmor);
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        amount += surplus.amount;
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.Owner.OnRoundEnd -= OnTrigger;
        return UniTask.CompletedTask;
    }
}
