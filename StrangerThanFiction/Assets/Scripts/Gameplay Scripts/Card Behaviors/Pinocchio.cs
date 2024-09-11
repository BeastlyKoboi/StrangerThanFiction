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
    public override uint Id => 5;

    protected override Task SummonEffect()
    {
        OnRoundStart += CreateTallTaleInHand;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        OnRoundStart -= CreateTallTaleInHand;
        return Task.CompletedTask;
    }

    private Task CreateTallTaleInHand()
    {
        CardModel card = CardFactory.Instance.CreateCard("TallTale", IsHidden, gameObject.transform, Owner, Board);
        Owner.handManager.AddCardToHandFromDeck(card);
        return Task.CompletedTask;
    }
}
