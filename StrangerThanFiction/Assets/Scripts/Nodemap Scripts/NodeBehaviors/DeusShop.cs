using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeusShop : SpecialNode
{
    private GameData gameData;
    [SerializeField] private RunInfo runInfo;

    public bool isShopOpen = false;
    public bool hasOpenedShop = false;

    private ShopUI shopUI;

    private void Start()
    {
        mapNode.AddOnClick((MapNode mapNode) => OpenShop());


    }

    private void OpenShop()
    {
        isShopOpen = true;

        shopUI = nodeMenu.GetShopMenuUI();
        runInfo = nodeMenu.GetRunInfo();

        shopUI.OpenShop();

        shopUI.OnReroll.AddListener(RerollShop);
        shopUI.OnBuy.AddListener(BuyCard);
        
        if (!hasOpenedShop)
        {
            PopulateShop();
            hasOpenedShop = true;
        }
    }

    private async UniTask RerollShop()
    {
        if (runInfo.GetRerollTokens() <= 0) return;

        await runInfo.SetRerollTokens(runInfo.GetRerollTokens() - 1);
        IncrementRerollTokensUsed();

        PopulateShop();
    }

    private void PopulateShop() {
        List<DeckEntry> purchaseableCards = new List<DeckEntry>();
        List<int> prices = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            purchaseableCards.Add(nodeMenu.GetCardShop().GetNextPurchaseableCard());
            purchaseableCards[i].price = nodeMenu.GetCardShop().CalculateCardPrice(purchaseableCards[i]);
        }

        shopUI.PopulateShop(purchaseableCards);
    }

    private async UniTask BuyCard()
    {
        if (shopUI.SelectedSlot == null) return;
        if (runInfo.GetCurrency() < shopUI.SelectedSlot.GetPrice())
        {
            Debug.Log("Not enough currency");
            return;
        }

        await runInfo.SetCurrency(runInfo.GetCurrency() - shopUI.SelectedSlot.GetPrice());

        await runInfo.AddCardToDeckInventory(shopUI.SelectedSlot.GetDeckEntry());

        Destroy(shopUI.SelectedSlot.gameObject);
    }

    public override void LoadData(GameData data)
    {
        this.gameData = data;
    }

    public override void SaveData(GameData data)
    {

    }

}
