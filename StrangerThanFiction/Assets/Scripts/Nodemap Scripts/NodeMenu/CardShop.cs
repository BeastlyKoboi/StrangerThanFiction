using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    [SerializeField] private CardDictionary cardDictionary;
    [SerializeField] private ItemDictionary itemDictionary;

    [SerializeField] private List<CardInfo> collectibleCards;

    [SerializeField] private int baseCardPrice; 

    [SerializeField] private int baseCommonItemPrice;
    [SerializeField] private int baseUncommonItemPrice;
    [SerializeField] private int baseRareItemPrice;
    [SerializeField] private int baseEpicItemPrice;

    private void Awake()
    {
        //Random.InitState(10);
        Random.State state = Random.state;
        string stateSerialized = JsonUtility.ToJson(state);
        Debug.Log(stateSerialized);
        float random = Random.Range(0, 1);

        Debug.Log(random);

        collectibleCards = new List<CardInfo>();
        for (int i = 0; i < cardDictionary.GetEntries().Count; i++)
        {
            if (cardDictionary.GetEntries()[i].Value.CollectionType == CollectionType.Collectible)
            {
                collectibleCards.Add(cardDictionary.GetEntries()[i].Value);
            }
        }

    }

    public DeckEntry GetNextPurchaseableCard()
    {
        DeckEntry deckEntry = new DeckEntry();
        deckEntry.cardName = collectibleCards[Random.Range(0, collectibleCards.Count)].name;
        deckEntry.numCopies = 1;
        if (Random.Range(0f, 1f) < 0.5f)
        {
            deckEntry.items.Add(itemDictionary.GetEntries()[Random.Range(0, itemDictionary.GetEntries().Count)].Key);
        }
        return deckEntry;
    }

    public int CalculateCardPrice(DeckEntry deckEntry)
    {
        return baseCardPrice + deckEntry.items.Count * baseCommonItemPrice;
    }


}
