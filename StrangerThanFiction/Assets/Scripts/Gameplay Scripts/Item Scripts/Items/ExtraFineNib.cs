using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraFineNib : Item
{
    public ExtraFineNib(ItemInfo itemInfo, CardModel card) : base(itemInfo, card) { }

    public async override UniTask OnAdd()
    {
        await card.GrantCostModification(-2);
    }
}
