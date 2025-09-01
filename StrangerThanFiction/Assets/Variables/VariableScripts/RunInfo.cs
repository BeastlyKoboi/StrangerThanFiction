using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/RunInfo")]
public class RunInfo : ScriptableObject
{
    [Header("Variables")]
    [SerializeField] private int currency;
    [SerializeField] private DeckInventory deckInventory;
    [SerializeField] private List<string> boonList;
     
    [Header("Currency Events")]
    public GameEventAsync OnAfterCurrencyChange;

    [Header("Deck Inventory Events")]
    public GameEventAsync OnAfterDeckInventoryChange;

    [Header("Boon Events")]
    public GameEventAsync OnAfterBoonListChange;

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
    public async UniTask AddItemToCardInDeckInventory(DeckEntry deckEntry, Item item)
    {
        if (deckInventory.GetDeckEntries().Find(entry => entry.cardName == deckEntry.cardName) is DeckEntry existingEntry)
        {
            deckEntry.items.Add(item.GetType().ToString());
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


    public async UniTask SetBoonList(List<string> value)
    {
        boonList = value;
        await OnAfterBoonListChange.InvokeAsync(new EventState());
    }

    public async UniTask AddBoon(string boonName)
    {
        boonList.Add(boonName);
        await OnAfterBoonListChange.InvokeAsync(new EventState());
    }

    public async UniTask RemoveBoon(string boonName)
    {
        boonList.Remove(boonName);
        await OnAfterBoonListChange.InvokeAsync(new EventState());
    }

    public List<string> GetBoonList() => boonList;


}
