using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform conditionsBox;
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI shopDescriptionText;
    [SerializeField] private Button rerollBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject cardZone;

    // Events 
    public UniTaskEvent OnReroll = new UniTaskEvent();
    public UniTaskEvent OnBuy = new UniTaskEvent();
    public UniTaskEvent OnClose = new UniTaskEvent();

    [SerializeField] private RunInfo runInfo;

    [SerializeField] private ItemDataMono itemData;

    [Header("Prefabs")]
    [SerializeField] private GameObject shopSlotPrefab;

    private ShopSlot selectedSlot;
    public ShopSlot SelectedSlot
    {
        get { return selectedSlot; }
    }

    private void Awake()
    {
        rerollBtn.onClick.AddListener(async () =>
        {
            rerollBtn.interactable = false;
            await OnReroll.InvokeAsync();
            rerollBtn.interactable = true;
        });

        buyBtn.onClick.AddListener(async () =>
        {
            buyBtn.interactable = false;
            await OnBuy.InvokeAsync();
            buyBtn.interactable = true;
        });

        closeBtn.onClick.AddListener(async () =>
        {
            await OnClose.InvokeAsync();

            CloseShop();
        });

        if (itemData == null)
            itemData = GameObject.Find("ItemData").GetComponent<ItemDataMono>();
    }


    public void PopulateShop(List<DeckEntry> purchaseableCards)
    {
        foreach (Transform child in cardZone.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < purchaseableCards.Count; i++)
        {
            ShopSlot shopSlot = Instantiate(shopSlotPrefab, cardZone.transform).GetComponent<ShopSlot>();
            CardModel card = CardFactory.Instance.CreateNonPlayableCard(purchaseableCards[i].cardName, false, shopSlot.GetCardParent());

            if (purchaseableCards[i].items.Count > 0)
            {
                ItemInfo itemInfo = itemData.itemDictionary.GetByKey(purchaseableCards[i].items[0]);
                Type itemScript = Type.GetType(purchaseableCards[i].items[0]);
                Item item = (Item)Activator.CreateInstance(itemScript, itemInfo, card);
                card.AddItem(item);
                shopSlot.SetItemDescText(item);
                shopSlot.ToggleItemDescText(true);
            }

            shopSlot.SetPriceText(purchaseableCards[i].price);
            shopSlot.SetDeckEntry(purchaseableCards[i]);

            card.GetComponent<Clickable>().OnLeftClick += (CardModel card) => { SelectSlot(shopSlot); };
        }

        buyBtn.interactable = false;
    }

    public void SelectSlot(ShopSlot shopSlot)
    {
        if (selectedSlot != null)
        {
            selectedSlot.ToggleSelected(false);
        }
        selectedSlot = shopSlot;
        selectedSlot.ToggleSelected(true);
        buyBtn.interactable = true;
    }

    

    public void OpenShop()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseShop()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

}
