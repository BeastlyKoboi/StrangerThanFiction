using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleNodeData", menuName = "DataContainer/BattleNodeData")]
public class BattleNodeData : NodeData
{
    
    public DeckInventory DeckInventory;

    public string BeforeCombatTransitionText;
    public string AfterCombatTransitionText;

}
