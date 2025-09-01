using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialNode : MonoBehaviour, IDataPersistence
{
    protected GameData gameData;
    protected NodeMenu nodeMenu;
    protected RunManager runManager;
    protected MapNode mapNode;
    protected RunInfo runInfo;

    protected EncounterUI nodeUI;

    public bool hasOpenedShop = false;
    public bool hasOpenedShopThisSession = false;

    protected int freeRerolls = 0;
    protected int rerollsUsed;

    private void Awake()
    {
        nodeMenu = FindObjectOfType<NodeMenu>();
        runManager = FindObjectOfType<RunManager>();
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
            hasOpenedShop = flatNode.flatNodeSpecial.hasOpenedShop;
            freeRerolls = flatNode.flatNodeSpecial.freeRerolls;
            rerollsUsed = flatNode.flatNodeSpecial.rerollsUsed;
        }
    }

    protected virtual void Open() {
        nodeUI = nodeMenu.GetNodeUI(this);
        nodeUI.OpenUI();

        AddListenersToUI();

        if (!hasOpenedShop)
        {
            runManager.BoonCollection.EnterEncounter(this);

            PopulateUI();
            hasOpenedShop = true;
            hasOpenedShopThisSession = true;
        }
        else if (!hasOpenedShopThisSession)
        {
            PopulateUI();
            hasOpenedShopThisSession = true;
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

    public virtual void IncrementFreeRerolls() { freeRerolls++; }
    public virtual void DecrementFreeRerolls() { freeRerolls--; }

    public virtual void IncrementRerollsUsed() { rerollsUsed++; }
    public virtual void DecrementRerollsUsed() { rerollsUsed--; }
    public int GetRerollsUsed() => rerollsUsed;
    protected virtual async UniTask Reroll()
    {
        if (freeRerolls > 0)
        {
            DecrementFreeRerolls();
            IncrementRerollsUsed();
            PopulateUI();
            return;
        }

        if (runInfo.GetCurrency() < GetNextRerollCost()) return;

        await runInfo.SetCurrency(runInfo.GetCurrency() - GetNextRerollCost());

        IncrementRerollsUsed();

        PopulateUI();
    }

    public int GetNextRerollCost()
    {
        if (freeRerolls > 0) return 0;

        int nextRerollCost = 1;
        for (int i = 0; i < rerollsUsed; i++)
        {
            nextRerollCost *= 2;
        }
        return nextRerollCost;
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
            hasOpenedShop = hasOpenedShop,
            freeRerolls = freeRerolls,
            rerollsUsed = rerollsUsed
        };
    }
}
