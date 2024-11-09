using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeData", menuName = "DataContainer/NodeData")]
public class NodeData : ScriptableObject
{
    public string Title;
    [TextArea] public string Description;
    public Sprite Icon;

}
