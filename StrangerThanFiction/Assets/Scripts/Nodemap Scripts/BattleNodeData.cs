using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleNodeData", menuName = "DataContainer/BattleNodeData")]
public class BattleNodeData : NodeData
{
    public BattleNodeType Type;
    public DeckInventory DeckInventory;

    public string[] Boons;

    public string BeforeCombatTransitionText;
    public string AfterCombatTransitionText;

}
public enum BattleNodeType { Boss, Normal };