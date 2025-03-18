using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialNode : MonoBehaviour, IDataPersistence
{
    protected NodeMenu nodeMenu;
    protected MapNode mapNode;

    //
    protected int rerollTokensUsed;

    private void Awake()
    {
        nodeMenu = FindObjectOfType<NodeMenu>();
        mapNode = GetComponent<MapNode>();
    }

    public void Initialize(SpecialNodeData nodeData, GameData gameData, FlatNode flatNode = null)
    {
        if (flatNode != null)
        {
            rerollTokensUsed = flatNode.flatNodeSpecial.rerollTokensUsed;
        }
    }

    public virtual void IncrementRerollTokensUsed() { rerollTokensUsed++; }
    public virtual void DecrementRerollTokensUsed() { rerollTokensUsed--; }
    public int GetRerollTokensUsed() => rerollTokensUsed;

    public abstract void LoadData(GameData data);
    public abstract void SaveData(GameData data);

    public FlatNodeSpecial GetFlatNodeSpecial()
    {
        return new FlatNodeSpecial
        {
            rerollTokensUsed = rerollTokensUsed
        };
    }
}
