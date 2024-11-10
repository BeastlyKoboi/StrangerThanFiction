using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AnimatedArtisan : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnRoundStart += RoundStartEffect;
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart -= RoundStartEffect;
        return UniTask.CompletedTask;
    }

    private UniTask RoundStartEffect()
    {
        Owner.CreateCardInDeck(typeof(MagicalWoodcarving).ToString());
        return UniTask.CompletedTask;
    }
}
