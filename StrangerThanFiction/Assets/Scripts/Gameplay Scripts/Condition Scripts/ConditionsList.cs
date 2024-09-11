using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionsList", menuName = "Variables/ConditionsList")]
public class ConditionsList : ScriptableObject
{
    public ConditionInfo[] conditions;
}
