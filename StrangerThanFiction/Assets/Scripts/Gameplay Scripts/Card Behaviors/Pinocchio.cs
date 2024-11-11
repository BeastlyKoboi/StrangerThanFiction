using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Represents the functionality of Pinocchio Level 1 form 
/// </summary>
public sealed class Pinocchio : CardModel
{
    protected override UniTask SummonEffect()
    {
        OnRoundStart.AddListener(CreateTallTaleInHand);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundStart.RemoveListener(CreateTallTaleInHand);
        return UniTask.CompletedTask;
    }

    private UniTask CreateTallTaleInHand()
    {
        Owner.CreateCardInHand("TallTale");
        return UniTask.CompletedTask;
    }
}
