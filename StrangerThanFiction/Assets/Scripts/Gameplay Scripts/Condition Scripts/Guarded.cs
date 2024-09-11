using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Guarded : Condition
{
    public override uint Id => 3;
    public Guarded(CardModel card, int amount) : base(card, amount) { }

    public override Task OnAdd()
    {
        card.Owner.OnRoundEnd += OnTrigger;
        return Task.CompletedTask;
    }
    public override async Task OnTrigger()
    {
        if (card.CurrentPlotArmor < amount)
            await card.GrantPlotArmor(amount - card.CurrentPlotArmor);
    }
    public override Task OnSurplus(Condition surplus)
    {
        amount += surplus.amount;
        return Task.CompletedTask;
    }
    public override Task OnRemove()
    {
        card.Owner.OnRoundEnd -= OnTrigger;
        return Task.CompletedTask;
    }
}
