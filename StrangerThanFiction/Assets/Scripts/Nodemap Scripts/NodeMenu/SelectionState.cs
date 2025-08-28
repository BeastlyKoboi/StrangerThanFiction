using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionState
{
    public SelectableSlot newSelection;
    public ShopSlot shopSlot;
    public ItemSlot itemSlot;
    public BoonSlot boonSlot;

    public SelectionResult selectionResult;

    public SelectionState(SelectableSlot newSelection, 
        ShopSlot shopSlot = null, 
        ItemSlot itemSlot = null, 
        BoonSlot boonSlot = null, 
        SelectionResult selectionResult = null) 
    { 
        this.newSelection = newSelection;
        this.shopSlot = shopSlot;
        this.itemSlot = itemSlot;
        this.boonSlot = boonSlot;
        this.selectionResult = selectionResult;
    }
}
