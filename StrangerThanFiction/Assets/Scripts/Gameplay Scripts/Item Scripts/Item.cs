using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Item : ISelectable
{
    protected ItemInfo itemInfo;
    protected CardModel card;

    public virtual Sprite Icon { get; set; }

    public Item(ItemInfo itemInfo, CardModel card)
    {
        this.itemInfo = itemInfo;
        this.card = card;
        this.Icon = itemInfo.Image;
    }

    public virtual UniTask OnAdd() => UniTask.CompletedTask;
    public virtual UniTask OnRemove() => UniTask.CompletedTask;
    public override string ToString() => $"<line-indent=30>{itemInfo.ItemName}: {itemInfo.Description}";
    public virtual bool IsCardCompatible(CardModel card) => itemInfo.IsCardCompatible(card);
}
