using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardInfo", menuName = "DataContainer/CardInfo")]
public class CardInfo : ScriptableObject
{
    public string Title;
    [TextArea]
    public string Description;
    [TextArea]
    public string FlavorText;
    public CardType Type;
    public string PortraitPath;
    public int BaseCost;
    public int BasePower;
    public int BasePlotArmor;
}