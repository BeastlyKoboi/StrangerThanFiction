using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardInfo", menuName = "DataContainer/CardInfo")]
public class CardInfo : ScriptableObject
{
    public Sprite Portrait;
    public string Title;
    [TextArea]
    public string Description;
    [TextArea]
    public string FlavorText;
    public CardType Type;
    public Faction Faction;
    public CollectionType CollectionType;
    public int BaseCost;
    public int BasePower;
    public int BasePlotArmor;
    public PlayRequirements PlayRequirements;

    [TextArea] public string EncounterText;
}