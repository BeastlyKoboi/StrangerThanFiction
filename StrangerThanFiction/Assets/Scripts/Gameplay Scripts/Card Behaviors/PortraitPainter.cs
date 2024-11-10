using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PortraitPainter : CardModel
{
    protected override UniTask SummonEffect()
    {
        Owner.OnCardDrawn += OnDrawEffect;
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnCardDrawn -= OnDrawEffect;
        return UniTask.CompletedTask;
    }

    private async UniTask OnDrawEffect(CardModel card)
    {
        if (card.Type == CardType.Unit)
        {
            await card.GrantPower(2);
        }
    }

}
