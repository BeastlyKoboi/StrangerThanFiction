using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOfCharacter : SpecialNode
{
    protected override void PopulateUI()
    {
        List<DeckEntry> deckEntries = runInfo.GetDeckInventory().GetDeckEntries();
        CardShop cardShop = nodeMenu.GetCardShop();
        PopulateUIState state = new PopulateUIState();

        List<string> availableCards = new List<string>();

        foreach (DeckEntry entry in deckEntries)
        {
            for (int i = 0; i < entry.numCopies; i++)
            {
                availableCards.Add(entry.cardName);
            }
        }

        // Select up to 5 different cards
        HashSet<string> selectedCardNames = new HashSet<string>();
        List<DeckEntry> selectedDeckEntries = new List<DeckEntry>();

        int maxCards = Mathf.Min(5, deckEntries.Count);
        for (int i = 0; i < maxCards && availableCards.Count > 0; i++)
        {
            string randomCard = availableCards[Random.Range(0, availableCards.Count)];
            if (selectedCardNames.Contains(randomCard))
            {
                availableCards.RemoveAll(card => card == randomCard);
                i--;
                continue;
            }
            DeckEntry selectedDeckEntry = deckEntries.Find(entry => entry.cardName == randomCard);
            if (selectedDeckEntry != null)
            {
                selectedCardNames.Add(randomCard);
                selectedDeckEntries.Add(selectedDeckEntry);
            }
            availableCards.RemoveAll(card => card == randomCard);
        }

        // Always pick 3 different items, one per card (if possible)
        HashSet<string> selectedItemNames = new HashSet<string>();
        List<ItemInfo> selectedItems = new List<ItemInfo>();
        int itemsNeeded = Mathf.Min(3, selectedDeckEntries.Count);

        // Try to pick 3 unique items, each compatible with a different card
        for (int i = 0; i < selectedDeckEntries.Count && selectedItems.Count < itemsNeeded; i++)
        {
            DeckEntry deckEntry = selectedDeckEntries[i];
            ItemInfo selectedItem = null;
            int attempts = 0;
            // Try up to 10 times to get a unique item for this card
            while (attempts < 10)
            {
                selectedItem = cardShop.GetRandomItem(deckEntry.cardName);
                if (selectedItem != null && !selectedItemNames.Contains(selectedItem.name))
                {
                    selectedItemNames.Add(selectedItem.name);
                    selectedItems.Add(selectedItem);
                    break;
                }
                attempts++;
            }
        }

        // Add card options
        foreach (DeckEntry deckEntry in selectedDeckEntries)
        {
            state.options.Add(new EncounterOption(EncounterOptionType.Card, deckEntry));
        }

        // Add exactly 3 item options (or less if not enough unique items found)
        for (int i = 0; i < selectedItems.Count; i++)
        {
            state.options.Add(new EncounterOption(EncounterOptionType.Item, selectedItems[i]));
        }

        nodeUI.PopulateUI(state);
    }

    protected override UniTask CheckSelection(SelectionState selectionState)
    {
        TestOfCharacterUI testOfCharacterUI = nodeUI as TestOfCharacterUI;

        // If neither slot is selected, cannot confirm or select
        if (selectionState.shopSlot == null && selectionState.itemSlot == null)
        {
            selectionState.selectionResult.AllowSelection = true;
            selectionState.selectionResult.ConfirmInteractable = false;
            return UniTask.CompletedTask;
        }

        // If selecting a card, and an item is already selected, check compatibility
        if (selectionState.newSelection is ShopSlot newShopSlot && selectionState.itemSlot != null)
        {
            bool compatible = selectionState.itemSlot.item.IsCardCompatible(newShopSlot.CardModel);
            testOfCharacterUI.SetCompatibilityLabel(compatible);
            selectionState.selectionResult.AllowSelection = true;
            selectionState.selectionResult.ConfirmInteractable = compatible;
            return UniTask.CompletedTask;
        }

        // If selecting an item, and a card is already selected, check compatibility
        if (selectionState.newSelection is ItemSlot newItemSlot && selectionState.shopSlot != null)
        {
            bool compatible = newItemSlot.item.IsCardCompatible(selectionState.shopSlot.CardModel);
            testOfCharacterUI.SetCompatibilityLabel(compatible);
            selectionState.selectionResult.AllowSelection = true;
            selectionState.selectionResult.ConfirmInteractable = compatible;
            return UniTask.CompletedTask;
        }

        // If a single slot is selected twice, just allow selection but not confirmation
        selectionState.selectionResult.AllowSelection = true;
        selectionState.selectionResult.ConfirmInteractable = false;

        return UniTask.CompletedTask;
    }

    protected override async UniTask Confirm(ConfirmSelectState confirmSelectState)
    {
        if (confirmSelectState.shopSlot == null || confirmSelectState.itemSlot == null) return;

        DeckEntry deckEntry = confirmSelectState.shopSlot.GetDeckEntry();
        Item item = confirmSelectState.itemSlot.item;

        await runInfo.AddItemToCardInDeckInventory(deckEntry, item);

        nodeUI.CloseUI();
        mapNode.ToggleSelectable(false);
    }
}
