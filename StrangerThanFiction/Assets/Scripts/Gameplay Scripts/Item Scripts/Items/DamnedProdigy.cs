using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DamnedProdigy : Item
{
    public DamnedProdigy(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {

    }

    public override UniTask OnAdd()
    {
        card.OnSummon.AddListener(GrantKeywords);
        return base.OnAdd();
    }

    public override UniTask OnRemove()
    {
        card.OnSummon.RemoveListener(GrantKeywords);
        return base.OnRemove();
    }

    private async UniTask GrantKeywords()
    {
        await card.ApplyCondition(new Doomed(card, 1));
        await card.ApplyCondition(new Fated(card, 1));
    }
}
