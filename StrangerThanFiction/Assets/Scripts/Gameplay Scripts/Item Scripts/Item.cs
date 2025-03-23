using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Item 
{
    protected ItemInfo itemInfo;
    protected CardModel card;

    public Item(ItemInfo itemInfo, CardModel card)
    {
        this.itemInfo = itemInfo;
        this.card = card;
    }

    public virtual UniTask OnAdd() => UniTask.CompletedTask;
    public virtual UniTask OnRemove() => UniTask.CompletedTask;
    public override string ToString() => $"{itemInfo.ItemName}: {itemInfo.Description}";
}
