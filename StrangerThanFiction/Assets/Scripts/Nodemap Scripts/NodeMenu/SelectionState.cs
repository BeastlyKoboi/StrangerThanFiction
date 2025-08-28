using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionState
{
    public SelectableSlot newSelection;
    public ShopSlot shopSlot;
    public ItemSlot itemSlot;

    public SelectionResult selectionResult;

    public SelectionState(SelectableSlot newSelection, ShopSlot shopSlot = null, ItemSlot itemSlot = null, SelectionResult selectionResult = null) 
    { 
        this.newSelection = newSelection;
        this.itemSlot = itemSlot;
        this.shopSlot = shopSlot;
        this.selectionResult = selectionResult;
    }
}
