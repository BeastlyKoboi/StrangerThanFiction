using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheCuttingRoom : SpecialNode
{
    protected override void PopulateUI()
    {
        List<DeckEntry> deck = runInfo.GetDeckInventory().GetDeckEntries().ToList();
        PopulateUIState state = new PopulateUIState();

        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, deck.Count - 1);
            state.options.Add(new EncounterOption(EncounterOptionType.Card, deck[randomIndex]));
            
            deck.RemoveAt(randomIndex);
        }

        state.rerollCost = GetNextRerollCost();

        nodeUI.PopulateUI(state);
    }

    protected override async UniTask Confirm(ConfirmSelectState confirmSelectState)
    {
        if (nodeUI.SelectedCardSlot == null) return;

        DeckEntry cardToRemove = nodeUI.SelectedCardSlot.GetDeckEntry();

        await runInfo.RemoveCardFromDeckInventory(cardToRemove.cardName);

        Destroy(nodeUI.SelectedCardSlot.gameObject);
    }
}
