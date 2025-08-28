using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "DataContainer/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public string ItemName;
    [TextArea]
    public string Description;
    public Sprite Image;
    public Rarity ItemRarity;
    public ItemRequirement Requirement;

    public bool IsCardCompatible(CardModel card)
    {
        if (card == null) return false;
        if (Requirement == ItemRequirement.None) return true;
        if (Requirement == ItemRequirement.Unit && card.Type == CardType.Unit) return true;
        if (Requirement == ItemRequirement.Spell && card.Type == CardType.Spell) return true;

        return false;
    }
}

public enum ItemRequirement
{
    None,
    Unit,
    Spell,
}
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic
}

