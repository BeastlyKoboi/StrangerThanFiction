using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class EncounterUI : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected TextMeshProUGUI shopNameText;
    [SerializeField] protected TextMeshProUGUI shopDescriptionText;
    [SerializeField] protected Button rerollBtn;
    [SerializeField] protected Button confirmBtn;
    [SerializeField] protected Button closeBtn;
    [SerializeField] protected GameObject cardZone;
    [SerializeField] protected GameObject itemZone;
    [SerializeField] protected GameObject boonZone; 

    // Events 
    public UniTaskEvent OnReroll = new UniTaskEvent();
    public UniTaskEvent<SelectionState> OnSelect = new UniTaskEvent<SelectionState>();
    public UniTaskEvent<ConfirmSelectState> OnConfirm = new UniTaskEvent<ConfirmSelectState>();
    public UniTaskEvent OnClose = new UniTaskEvent();

    [SerializeField] protected RunInfo runInfo;

    [SerializeField] protected ItemDataMono itemData;

    [Header("Prefabs")]
    [SerializeField] protected GameObject shopSlotPrefab;
    [SerializeField] protected GameObject itemSlotPrefab;
    [SerializeField] protected GameObject boonSlotPrefab; 
    [SerializeField] protected Transform conditionsBox;

    protected ShopSlot selectedCardSlot;
    public ShopSlot SelectedCardSlot
    {
        get => selectedCardSlot;
        private set { }
    }

    protected ItemSlot selectedItemSlot;
    public ItemSlot SelectedItemSlot 
    {
        get => selectedItemSlot;
        private set { }
    }

    protected BoonSlot selectedBoonSlot;
    public BoonSlot SelectedBoonSlot
    { 
        get => selectedBoonSlot;
        private set { }
    }

    private void Awake()
    {
        rerollBtn.onClick.AddListener(async () =>
        {
            rerollBtn.interactable = false;
            await OnReroll.InvokeAsync();
            rerollBtn.interactable = true;
        });

        confirmBtn.onClick.AddListener(async () =>
        {
            confirmBtn.interactable = false;
            await OnConfirm.InvokeAsync(new ConfirmSelectState(shopSlot: selectedCardSlot, itemSlot: selectedItemSlot, boonSlot: selectedBoonSlot));
            confirmBtn.interactable = true;
        });

        closeBtn.onClick.AddListener(async () =>
        {
            await OnClose.InvokeAsync();

            CloseUI();
        });

        if (itemData == null)
            itemData = GameObject.Find("ItemData").GetComponent<ItemDataMono>();
    }

    public virtual void PopulateUI(PopulateUIState populateUIState)
    {
        // Clear all zones
        if (cardZone != null)
            foreach (Transform child in cardZone.transform) Destroy(child.gameObject);
        if (itemZone != null)
            foreach (Transform child in itemZone.transform) Destroy(child.gameObject);
        if (boonZone != null)
            foreach (Transform child in boonZone.transform) Destroy(child.gameObject);

        Clickable clickable;

        foreach (EncounterOption option in populateUIState.options)
        {
            switch (option.OptionType)
            {
                case EncounterOptionType.Card:
                    DeckEntry deckEntry = option.Data as DeckEntry;
                    ShopSlot shopSlot = Instantiate(shopSlotPrefab, cardZone.transform).GetComponent<ShopSlot>();
                    CardModel card = CardFactory.Instance.CreateNonPlayableCard(deckEntry.cardName, false, shopSlot.GetCardParent());
                    shopSlot.ToggleItemDescText(false);

                    if (populateUIState.showNewCardItems && deckEntry.items.Count > 0)
                    {
                        ItemInfo itemInfo = itemData.itemDictionary.GetByKey(deckEntry.items[0]);
                        Type itemScript = Type.GetType(deckEntry.items[0]);
                        Item item = (Item)Activator.CreateInstance(itemScript, itemInfo, card);
                        card.AddItem(item);
                        shopSlot.SetItemDescText(item);
                        shopSlot.ToggleItemDescText(true);
                    }

                    if (populateUIState.showPrices)
                        shopSlot.SetPriceText(deckEntry.price);
                    else
                        shopSlot.TogglePriceText(false);

                    shopSlot.SetDeckEntry(deckEntry);
                    shopSlot.CardModel = card;

                    card.GetComponent<Clickable>().OnLeftClick += (ISelectable selectable) => { SelectCardSlot(shopSlot); };
                    break;

                case EncounterOptionType.Item:
                    ItemInfo itemInfoOption = option.Data as ItemInfo;
                    ItemSlot itemSlot = Instantiate(itemSlotPrefab, itemZone.transform).GetComponent<ItemSlot>();
                    Type itemType = Type.GetType(itemInfoOption.name);
                    Item itemObj = (Item)Activator.CreateInstance(itemType, itemInfoOption, null);

                    itemSlot.SetItemDescText(itemObj);

                    clickable = itemSlot.GetComponent<Clickable>();

                    clickable.SetSelectableTarget(itemObj);
                    clickable.SetOnLeftClick((ISelectable selectable) => { SelectItemSlot(itemSlot); });

                    break;

                case EncounterOptionType.Boon:
                    if (boonZone == null || boonSlotPrefab == null) break;
                    BoonInfo boonInfo = option.Data as BoonInfo;
                    BoonSlot boonSlot = Instantiate(boonSlotPrefab, boonZone.transform).GetComponent<BoonSlot>();
                    Type boonType = Type.GetType(boonInfo.name);
                    Boon boon = (Boon)Activator.CreateInstance(boonType);
                    boonSlot.SetBoonText(boon);

                    clickable = boonSlot.GetComponent<Clickable>();

                    clickable.SetSelectableTarget(boon);
                    clickable.SetOnLeftClick((ISelectable selectable) => { SelectBoonSlot(boonSlot); });
                    break;
            }
        }

        string rerollText = populateUIState.rerollCost > 0 ? $"Reroll (${populateUIState.rerollCost})" : "Reroll (Free)";
        rerollBtn.GetComponentInChildren<TextMeshProUGUI>().text = rerollText;

        confirmBtn.interactable = false;
    }

    public virtual async void SelectCardSlot(ShopSlot shopSlot)
    {
        SelectionResult selectionResult = new SelectionResult(true, true);
        await OnSelect.InvokeAsync(new SelectionState(
            newSelection: shopSlot, 
            shopSlot: selectedCardSlot, 
            itemSlot: selectedItemSlot, 
            boonSlot: selectedBoonSlot,
            selectionResult: selectionResult));

        if (selectionResult == null || !selectionResult.AllowSelection)
            return;

        if (selectedCardSlot != null)
            selectedCardSlot.ToggleSelected(false);

        selectedCardSlot = shopSlot;
        selectedCardSlot.ToggleSelected(true);
        confirmBtn.interactable = selectionResult.ConfirmInteractable;
    }

    public virtual async void SelectItemSlot(ItemSlot itemSlot)
    {
        SelectionResult selectionResult = new SelectionResult(true, true);
        await OnSelect.InvokeAsync(new SelectionState(
            newSelection: itemSlot, 
            shopSlot: selectedCardSlot, 
            itemSlot: selectedItemSlot, 
            boonSlot: selectedBoonSlot,
            selectionResult: selectionResult));

        if (selectionResult == null || !selectionResult.AllowSelection)
            return;

        if (selectedItemSlot != null)
            selectedItemSlot.ToggleSelected(false);

        selectedItemSlot = itemSlot;
        selectedItemSlot.ToggleSelected(true);
        confirmBtn.interactable = selectionResult.ConfirmInteractable;
    }

    public virtual async void SelectBoonSlot(BoonSlot boonSlot)
    {
        SelectionResult selectionResult = new SelectionResult(true, true);
        await OnSelect.InvokeAsync(new SelectionState(
            newSelection: boonSlot, 
            shopSlot: selectedCardSlot, 
            itemSlot: selectedItemSlot, 
            boonSlot: selectedBoonSlot,
            selectionResult: selectionResult));
        if (selectionResult == null || !selectionResult.AllowSelection)
            return;
        if (selectedBoonSlot != null)
            selectedBoonSlot.ToggleSelected(false);
        selectedBoonSlot = boonSlot;
        selectedBoonSlot.ToggleSelected(true);
        confirmBtn.interactable = selectionResult.ConfirmInteractable;
    }

    public void OpenUI()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
