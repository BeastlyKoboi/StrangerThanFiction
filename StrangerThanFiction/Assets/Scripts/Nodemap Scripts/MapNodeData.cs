using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Battle, Special
}

[CreateAssetMenu(fileName = "MapNodeData", menuName = "DataContainer/MapNodeData")]
public class MapNodeData : ScriptableObject
{
    public string Title;
    [TextArea] public string Description;
    public NodeType Type;
    public Sprite Icon;
    public DeckInventory DeckInventory;

}
