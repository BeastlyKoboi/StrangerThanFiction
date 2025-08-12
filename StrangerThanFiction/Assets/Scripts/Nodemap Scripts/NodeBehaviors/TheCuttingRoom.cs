using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheCuttingRoom : SpecialNode
{ 
    protected override UniTask AddListenersToUI()
    {
        nodeUI.OnBuy.AddListener(RemoveCard);
        nodeUI.OnReroll.AddListener(Reroll);
        nodeUI.OnClose.AddListener(RemoveListenersFromUI);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveListenersFromUI()
    {
        nodeUI.OnBuy.RemoveListener(RemoveCard);
        nodeUI.OnReroll.RemoveListener(Reroll);
        nodeUI.OnClose.RemoveListener(RemoveListenersFromUI);
        return UniTask.CompletedTask;
    }

    protected override void PopulateUI()
    {
        List<DeckEntry> removeableCards = new List<DeckEntry>();
        List<int> prices = new List<int>();

        List<DeckEntry> deck = runInfo.GetDeckInventory().GetDeckEntries().ToList();

        for (int i = 0; i < 5; i++)
        {
            int randomIndex = Random.Range(0, deck.Count - 1);
            removeableCards.Add(deck[randomIndex]);
            deck.RemoveAt(randomIndex);
        }

        nodeUI.PopulateShop(removeableCards, showItems: false);
    }

    private async UniTask RemoveCard()
    {
        if (nodeUI.SelectedSlot == null) return;


        DeckEntry cardToRemove = nodeUI.SelectedSlot.GetDeckEntry();

        await runInfo.RemoveCardFromDeckInventory(cardToRemove.cardName);

        Destroy(nodeUI.SelectedSlot.gameObject);
    }

    private async UniTask Reroll()
    {
        if (runInfo.GetRerollTokens() <= 0) return;

        await runInfo.SetRerollTokens(runInfo.GetRerollTokens() - 1);
        IncrementRerollTokensUsed();

        PopulateUI();
    }
}
