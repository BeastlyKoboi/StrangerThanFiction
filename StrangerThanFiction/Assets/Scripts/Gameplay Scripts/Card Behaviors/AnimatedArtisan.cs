using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatedArtisan : CardModel
{
    public override uint Id => 0;

    protected override Task SummonEffect()
    {
        Owner.OnRoundStart += RoundStartEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
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
