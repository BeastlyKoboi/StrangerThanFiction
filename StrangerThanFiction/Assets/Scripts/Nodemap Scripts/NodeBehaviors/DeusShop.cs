using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DeusShop : SpecialNode
{
    protected override void PopulateUI() {
        PopulateUIState state = new PopulateUIState();

        for (int i = 0; i < 5; i++)
        {
            DeckEntry entry = nodeMenu.GetCardShop().GetNextPurchaseableCard();
            entry.price = nodeMenu.GetCardShop().CalculateCardPrice(entry);

            state.options.Add(new EncounterOption(EncounterOptionType.Card, entry));
        }

        state.showPrices = true;
        state.showNewCardItems = true;
        state.rerollCost = GetNextRerollCost();

        nodeUI.PopulateUI(state);
    }

    protected override async UniTask Confirm(ConfirmSelectState confirmSelectState)
    {
        if (nodeUI.SelectedCardSlot == null) return;
        if (runInfo.GetCurrency() < nodeUI.SelectedCardSlot.GetPrice())
        {
            Debug.Log("Not enough currency");
            return;
        }

        await runInfo.SetCurrency(runInfo.GetCurrency() - nodeUI.SelectedCardSlot.GetPrice());

        await runInfo.AddCardToDeckInventory(nodeUI.SelectedCardSlot.GetDeckEntry());

        Destroy(nodeUI.SelectedCardSlot.gameObject);
    }
}
