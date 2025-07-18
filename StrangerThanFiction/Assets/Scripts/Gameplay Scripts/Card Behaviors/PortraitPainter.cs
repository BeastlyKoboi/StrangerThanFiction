using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class PortraitPainter : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnCardDrawn.AddListener(OnDrawEffect);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnCardDrawn.RemoveListener(OnDrawEffect);
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
