using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Resilient : Condition
{
    public static new string StaticName => typeof(Resilient).Name;
    public override string Name => StaticName;
    public override string Description { get; } = "Takes # less damage from all sources";

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
