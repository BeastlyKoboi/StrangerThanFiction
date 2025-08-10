using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class ProtectiveInstinct : Item
{
    public ProtectiveInstinct(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {
    }

    public override async UniTask OnAdd()
    {
        await card.GrantPlotArmor(3);
    }

}
