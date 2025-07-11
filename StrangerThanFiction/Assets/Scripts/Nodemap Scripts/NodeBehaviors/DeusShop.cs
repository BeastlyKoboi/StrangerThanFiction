using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeusShop : SpecialNode
{
    protected override UniTask AddListenersToUI()
    {
        nodeUI.OnReroll.AddListener(RerollShop);
        nodeUI.OnBuy.AddListener(BuyCard);
        nodeUI.OnClose.AddListener(RemoveListenersFromUI);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveListenersFromUI()
    {
        nodeUI.OnReroll.RemoveListener(RerollShop);
        nodeUI.OnBuy.RemoveListener(BuyCard);
        nodeUI.OnClose.RemoveListener(RemoveListenersFromUI);
        return UniTask.CompletedTask;
    }

    protected override void PopulateUI() {
        List<DeckEntry> purchaseableCards = new List<DeckEntry>();
        List<int> prices = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            purchaseableCards.Add(nodeMenu.GetCardShop().GetNextPurchaseableCard());
            purchaseableCards[i].price = nodeMenu.GetCardShop().CalculateCardPrice(purchaseableCards[i]);
        }

        nodeUI.PopulateShop(purchaseableCards);
    }

    private async UniTask RerollShop()
    {
        Debug.Log("Reroll shop called");

        if (runInfo.GetRerollTokens() <= 0) return;

        await runInfo.SetRerollTokens(runInfo.GetRerollTokens() - 1);
        IncrementRerollTokensUsed();

        PopulateUI();
    }

    private async UniTask BuyCard()
    {
        if (nodeUI.SelectedSlot == null) return;
        if (runInfo.GetCurrency() < nodeUI.SelectedSlot.GetPrice())
        {
            Debug.Log("Not enough currency");
            return;
        }

        await runInfo.SetCurrency(runInfo.GetCurrency() - nodeUI.SelectedSlot.GetPrice());

        await runInfo.AddCardToDeckInventory(nodeUI.SelectedSlot.GetDeckEntry());

        Destroy(nodeUI.SelectedSlot.gameObject);
    }

}
