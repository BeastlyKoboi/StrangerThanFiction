using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epiphany : Item
{ 
    public Epiphany(ItemInfo itemInfo, CardModel card) : base(itemInfo, card) { }

    public override UniTask OnAdd()
    {
        card.OnPlay.AddListener(DrawCard);
        return base.OnAdd();
    }

    public override UniTask OnRemove()
    {
        card.OnPlay.RemoveListener(DrawCard);
        return base.OnRemove();
    }

    private async UniTask DrawCard(CardPlayState cardPlayState)
    {
        await card.Owner.DrawCard();
    }
}
