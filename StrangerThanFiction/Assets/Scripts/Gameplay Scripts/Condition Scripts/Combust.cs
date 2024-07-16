using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Combust : ICondition
{
    private readonly CardModel card;
    public int amount;

    public Combust(CardModel card, int amount)
    {
        this.card = card;
        this.amount = amount;
    }

    public static string GetName()
    {
        return "Combust";
    }

    public Task OnAdd()
    {
        return Task.CompletedTask;
    }
    public Task OnTrigger()
    {
        return Task.CompletedTask;
    }
    public Task OnSurplus(ICondition surplus)
    {
        return Task.CompletedTask;
    }
    public Task OnRemove()
    {
        return Task.CompletedTask;
    }
}
