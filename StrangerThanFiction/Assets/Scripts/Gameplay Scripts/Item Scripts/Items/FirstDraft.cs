using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FirstDraft : Item
{
    public FirstDraft(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {
    }

    public override async UniTask OnAdd()
    {
        await card.ApplyCondition(new Combust(card, 0));
        card.OnPlay.AddListener(CreateCopyOfMeInHand);
    }

    public override UniTask OnRemove()
    {
        card.OnPlay.RemoveListener(CreateCopyOfMeInHand);
        return base.OnRemove();
    }

    private UniTask CreateCopyOfMeInHand(CardPlayState cardPlayState)
    {
        card.Owner.CreateCardInHand(cardPlayState.card.GetType().Name);
        return UniTask.CompletedTask;
    }
}
