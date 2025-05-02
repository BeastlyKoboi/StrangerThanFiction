using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FlatNode
{
    public string Title;
    public bool isSelectable;
    public bool isSelected;
    public string nodeDataKey;
    public FlatNodeSpecial flatNodeSpecial;
    public FlatNodeBattle flatNodeBattle;
}