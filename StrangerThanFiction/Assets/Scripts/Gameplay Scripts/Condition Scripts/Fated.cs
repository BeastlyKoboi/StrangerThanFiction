using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Fated : Condition
{
    public override uint Id => 2;
    public Fated(CardModel card, int amount) : base(card, amount) { }

    public override Task OnAdd()
    {
        card.Owner.OnRoundEnd += OnTrigger;
        return Task.CompletedTask;
    }
    public override async Task OnTrigger()
    {
        await card.GrantPower(amount);
    }
    public override Task OnSurplus(Condition surplus)
    {
        if (amount < surplus.amount)
            amount = surplus.amount;
        return Task.CompletedTask;
    }
    public override Task OnRemove()
    {
        card.Owner.OnRoundEnd += OnTrigger;
        return Task.CompletedTask;
    }
}
