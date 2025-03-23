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
}
