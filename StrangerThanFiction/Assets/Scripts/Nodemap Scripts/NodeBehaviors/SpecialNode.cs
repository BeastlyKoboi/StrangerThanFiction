using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialNode : MonoBehaviour, IDataPersistence
{
    protected NodeMenu nodeMenu;
    protected MapNode mapNode;

    private void Awake()
    {
        nodeMenu = FindObjectOfType<NodeMenu>();
        mapNode = GetComponent<MapNode>();
    }
    public abstract void LoadData(GameData data);
    public abstract void SaveData(GameData data);
}
