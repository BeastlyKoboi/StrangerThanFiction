using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Represents the functionality of Pinocchio Level 1 form 
/// </summary>
public sealed class Pinocchio : CardModel
{
    protected override Task SummonEffect()
    {
        OnRoundStart += CreateTallTaleInHand;
        return Task.CompletedTask;
    }

    protected override Task RemoveEffect(CardModel card)
    {
        OnRoundStart -= CreateTallTaleInHand;
        return Task.CompletedTask;
    }

    private Task CreateTallTaleInHand()
    {
        Owner.CreateCardInHand("TallTale");
        return Task.CompletedTask;
    }
}
