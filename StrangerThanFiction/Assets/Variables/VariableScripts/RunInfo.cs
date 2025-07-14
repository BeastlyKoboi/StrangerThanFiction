using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/RunInfo")]
public class RunInfo : ScriptableObject
{
    [Header("Variables")]
    [SerializeField] private int currency;
    [SerializeField] private int rerollTokens;
    [SerializeField] private DeckInventory deckInventory;

    [Header("Currency Events")]
    public GameEventAsync OnAfterCurrencyChange;

    [Header("Reroll Token Events")]
    public GameEventAsync OnAfterRerollTokensChange;

    [Header("Deck Inventory Events")]
    public GameEventAsync OnAfterDeckInventoryChange;

    public async UniTask SetCurrency(int value)
    {
        currency = value;
        await OnAfterCurrencyChange.InvokeAsync(new EventStateInt(value));
    }
    public async UniTask AddCurrency(int value)
    {
        currency += value;
        await OnAfterCurrencyChange.InvokeAsync(new EventStateInt(currency));
    }
    public async UniTask SubtractCurrency(int value)
    {
        currency -= value;
        await OnAfterCurrencyChange.InvokeAsync(new EventStateInt(currency));
    }
    public int GetCurrency() => currency;


    public async UniTask SetRerollTokens(int value)
    {
        rerollTokens = value;
        await OnAfterRerollTokensChange.InvokeAsync(new EventStateInt(value));
    }

    public int GetRerollTokens() => rerollTokens;



    public async UniTask SetDeckInventory(DeckInventory value)
    {
        deckInventory = value;
        await OnAfterDeckInventoryChange.InvokeAsync(new EventState());
    }
    public async UniTask AddCardToDeckInventory(DeckEntry newEntry)
    {
        if (deckInventory.GetDeckEntries().Find(entry => entry.cardName == newEntry.cardName) is DeckEntry deckEntry)
        {
            deckEntry.numCopies++;

            foreach (string item in newEntry.items)
            {
                if (!deckEntry.items.Contains(item))
                {
                    deckEntry.items.Add(item);
                }
            }
        }
        else
        {
            DeckEntry copiedEntry = new DeckEntry(newEntry.cardName, newEntry.numCopies);

            foreach (string item in newEntry.items)
            {
                copiedEntry.items.Add(item);
            }

            deckInventory.GetDeckEntries().Add(copiedEntry);
        }
        await OnAfterDeckInventoryChange.InvokeAsync(new EventState());
    }
    public async UniTask RemoveCardFromDeckInventory(string cardName)
    {
        if (deckInventory.GetDeckEntries().Find(entry => entry.cardName == cardName) is DeckEntry deckEntry)
        {
            deckEntry.numCopies--;
            if (deckEntry.numCopies <= 0)
            {
                deckInventory.GetDeckEntries().Remove(deckEntry);
            }
        }
        await OnAfterDeckInventoryChange.InvokeAsync(new EventState());
    }
    public DeckInventory GetDeckInventory() => deckInventory;

}
