using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Resilient : Condition
{
    public override uint Id => 4;
    public Resilient(CardModel card, int amount) : base(card, amount) { }

    public override Task OnAdd()
    {
        card.DamageResistence += amount;
        return Task.CompletedTask;
    }
    public override Task OnSurplus(Condition surplus)
    {
        if (amount < surplus.amount)
            amount = surplus.amount;
        return Task.CompletedTask;
    }
    public override Task OnRemove()
    {
        card.DamageResistence -= amount;
        return Task.CompletedTask;
    }
}
