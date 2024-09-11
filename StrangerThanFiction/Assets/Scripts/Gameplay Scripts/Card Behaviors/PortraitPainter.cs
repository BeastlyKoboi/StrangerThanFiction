using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PortraitPainter : CardModel
{
    public override uint Id => 6;

    protected override Task SummonEffect()
    {
        Owner.OnCardDrawn += OnDrawEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnCardDrawn -= OnDrawEffect;
        return Task.CompletedTask;
    }

    private async Task OnDrawEffect(CardModel card)
    {
        if (card.Type == CardType.Unit)
        {
            await card.GrantPower(2);
        }
    }

}
