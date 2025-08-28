using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterOptionType { Card, Item, Boon }

public class EncounterOption
{
    public EncounterOptionType OptionType;
    public object Data; // CardInfo, ItemInfo, BoonInfo, etc.
    public EncounterOption(EncounterOptionType type, object data)
    {
        OptionType = type;
        Data = data;
    }
}