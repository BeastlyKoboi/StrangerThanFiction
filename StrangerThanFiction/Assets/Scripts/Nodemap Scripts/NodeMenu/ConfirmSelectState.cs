using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmSelectState
{
    public ShopSlot shopSlot;
    public ItemSlot itemSlot;

    public ConfirmSelectState(ShopSlot shopSlot = null, ItemSlot itemSlot = null)
    {
        this.itemSlot = itemSlot;
        this.shopSlot = shopSlot;
    }
}
