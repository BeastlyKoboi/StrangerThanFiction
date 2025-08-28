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

    protected EncounterUI nodeUI;

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
        nodeUI.OpenUI();

        AddListenersToUI();

        if (!hasOpenedShop)
        {
            PopulateUI();
            hasOpenedShop = true;
        }
    }

    protected virtual UniTask AddListenersToUI()
    {
        nodeUI.OnConfirm.AddListener(Confirm);
        nodeUI.OnReroll.AddListener(Reroll);
        nodeUI.OnSelect.AddListener(CheckSelection);
        nodeUI.OnClose.AddListener(RemoveListenersFromUI);
        return UniTask.CompletedTask;
    }
    protected virtual UniTask RemoveListenersFromUI()
    {
        nodeUI.OnConfirm.RemoveListener(Confirm);
        nodeUI.OnReroll.RemoveListener(Reroll);
        nodeUI.OnSelect.RemoveListener(CheckSelection);
        nodeUI.OnClose.RemoveListener(RemoveListenersFromUI);
        return UniTask.CompletedTask;
    }

    protected abstract void PopulateUI();

    public virtual void IncrementRerollTokensUsed() { rerollTokensUsed++; }
    public virtual void DecrementRerollTokensUsed() { rerollTokensUsed--; }
    public int GetRerollTokensUsed() => rerollTokensUsed;
    protected virtual async UniTask Reroll()
    {
        if (runInfo.GetRerollTokens() <= 0) return;

        await runInfo.SetRerollTokens(runInfo.GetRerollTokens() - 1);
        IncrementRerollTokensUsed();

        PopulateUI();
    }
    protected virtual UniTask CheckSelection(SelectionState selectionState) 
        => UniTask.CompletedTask;
    protected abstract UniTask Confirm(ConfirmSelectState confirmSelectState);

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
