using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FineNib : Item
{
    public FineNib(ItemInfo itemInfo, CardModel card) : base(itemInfo, card) { }

    public async override UniTask OnAdd()
    {
        await card.GrantCostModification(-1);
    }
}
