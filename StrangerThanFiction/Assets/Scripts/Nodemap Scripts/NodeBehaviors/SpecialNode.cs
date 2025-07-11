using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialNode : MonoBehaviour, IDataPersistence
{
    protected GameData gameData;
    protected NodeMenu nodeMenu;
    protected MapNode mapNode;
    protected RunInfo runInfo;

    protected ShopUI nodeUI;

    public bool hasOpenedShop = false;

    protected int rerollTokensUsed;

    private void Awake()
    {
        nodeMenu = FindObjectOfType<NodeMenu>();
        mapNode = GetComponent<MapNode>();
        runInfo = nodeMenu.GetRunInfo();
    }

    protected virtual void Start()
    {
        mapNode.AddOnClick((MapNode mapNode) => Open());
    }

    public void Initialize(SpecialNodeData nodeData, GameData gameData, FlatNode flatNode = null)
    {
        if (flatNode != null)
        {
            rerollTokensUsed = flatNode.flatNodeSpecial.rerollTokensUsed;
        }
    }

    protected virtual void Open() {
        nodeUI = nodeMenu.GetNodeUI(this);
        nodeUI.OpenShop();

        AddListenersToUI();

        if (!hasOpenedShop)
        {
            PopulateUI();
            hasOpenedShop = true;
        }
    }

    protected abstract UniTask AddListenersToUI();
    protected abstract UniTask RemoveListenersFromUI();

    protected abstract void PopulateUI();

    public virtual void IncrementRerollTokensUsed() { rerollTokensUsed++; }
    public virtual void DecrementRerollTokensUsed() { rerollTokensUsed--; }
    public int GetRerollTokensUsed() => rerollTokensUsed;

    public virtual void LoadData(GameData data)
    {
        this.gameData = data;
    }

    public virtual void SaveData(GameData data) { }

    public FlatNodeSpecial GetFlatNodeSpecial()
    {
        return new FlatNodeSpecial
        {
            rerollTokensUsed = rerollTokensUsed
        };
    }
}
