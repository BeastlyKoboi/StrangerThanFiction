using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatedArtisan : CardModel
{
    protected override Task SummonEffect()
    {
        Owner.OnRoundStart += RoundStartEffect;
        return Task.CompletedTask;
    }

    protected override Task RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart -= RoundStartEffect;
        return Task.CompletedTask;
    }

    private Task RoundStartEffect()
    {
        Owner.CreateCardInDeck(typeof(MagicalWoodcarving).ToString());
        return Task.CompletedTask;
    }
}
