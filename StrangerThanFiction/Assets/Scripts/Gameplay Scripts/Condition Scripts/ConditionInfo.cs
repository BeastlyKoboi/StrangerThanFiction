using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionInfo", menuName = "DataContainer/ConditionInfo")]
public class ConditionInfo : ScriptableObject
{
    public string ConditionName;
    [TextArea]
    public string Description;
    public Sprite Image;
}