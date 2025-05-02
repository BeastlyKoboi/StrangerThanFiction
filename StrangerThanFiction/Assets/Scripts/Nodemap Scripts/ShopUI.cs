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
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI shopDescriptionText;
    [SerializeField] private Button rerollBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject cardZone;

    private CardShop cardShop;

    private DeusShop deusShop;

    [SerializeField] private RunInfo runInfo;

    [SerializeField] private ItemDataMono itemData;

    [Header("Prefabs")]
    [SerializeField] private GameObject shopSlotPrefab;

    private ShopSlot selectedSlot; 

    private void Awake()
    {
        cardShop = GetComponent<CardShop>();

        rerollBtn.onClick.AddListener(() =>
        {
            RerollShop().Forget();
        });

        buyBtn.onClick.AddListener(() =>
        {
            if (selectedSlot == null) return;

            if (runInfo.GetCurrency() < selectedSlot.GetPrice())
            {
                Debug.Log("Not enough currency");
                return;
            }

            runInfo.SetCurrency(runInfo.GetCurrency() - selectedSlot.GetPrice()).Forget();

            DeckEntry deckEntry = selectedSlot.GetDeckEntry();

            runInfo.AddCardToDeckInventory(deckEntry).Forget();
            
            buyBtn.interactable = false;

            Destroy(selectedSlot.gameObject);

        });

        closeBtn.onClick.AddListener(() =>
        {
            CloseShop();
        });

        if (itemData == null)
            itemData = GameObject.Find("ItemData").GetComponent<ItemDataMono>();
    }

    public void Initialize(DeusShop deusShop)
    {
        this.deusShop = deusShop;

        if (!deusShop.isShopOpen)
        {
            PopulateShop();
            deusShop.isShopOpen = true;
        }
    }

    public void PopulateShop()
    {
        foreach (Transform child in cardZone.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            ShopSlot shopSlot = Instantiate(shopSlotPrefab, cardZone.transform).GetComponent<ShopSlot>();

            DeckEntry cardEntry = cardShop.GetNextPurchaseableCard();

            CardModel card = CardFactory.Instance.CreateNonPlayableCard(cardEntry.cardName, false, shopSlot.GetCardParent());

            Debug.Log(cardEntry.items.Count);


            if (cardEntry.items.Count > 0)
            {
                Debug.Log(cardEntry.items[0]);

                ItemInfo itemInfo = itemData.itemDictionary.GetByKey(cardEntry.items[0]);
                Type itemScript = Type.GetType(cardEntry.items[0]);
                Item item = (Item)Activator.CreateInstance(itemScript, itemInfo, card);
                card.AddItem(item);
                shopSlot.SetItemDescText(item.ToString());
                shopSlot.ToggleItemDescText(true);
            }

            int price = cardShop.CalculateCardPrice(cardEntry);
            shopSlot.SetPriceText(price);
            shopSlot.SetDeckEntry(cardEntry);

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

    public async UniTask RerollShop()
    {
        if (runInfo.GetRerollTokens() <= 0) return; 

        await runInfo.SetRerollTokens(runInfo.GetRerollTokens() - 1);
        deusShop.IncrementRerollTokensUsed();
        PopulateShop();
    }


    public void CloseShop()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

}
