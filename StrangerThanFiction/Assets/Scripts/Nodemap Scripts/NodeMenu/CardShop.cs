using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    [SerializeField] private CardDictionary cardDictionary;
    [SerializeField] private ItemDictionary itemDictionary;

    [SerializeField] private List<CardInfo> collectibleCards;

    [SerializeField] private List<ItemInfo> commonUnitItems;
    [SerializeField] private List<ItemInfo> uncommonUnitItems;
    [SerializeField] private List<ItemInfo> rareUnitItems;
    [SerializeField] private List<ItemInfo> epicUnitItems;

    [SerializeField] private List<ItemInfo> commonSpellItems;
    [SerializeField] private List<ItemInfo> uncommonSpellItems;
    [SerializeField] private List<ItemInfo> rareSpellItems;
    [SerializeField] private List<ItemInfo> epicSpellItems;



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

        for (int i = 0; i < itemDictionary.GetEntries().Count; i++)
        {
            ItemInfo itemInfo = itemDictionary.GetEntries()[i].Value;
            if (itemInfo.Requirement == ItemRequirement.None || itemInfo.Requirement == ItemRequirement.Unit)
            {
                switch (itemInfo.ItemRarity)
                {
                    case Rarity.Common:
                        commonUnitItems.Add(itemInfo);
                        break;
                    case Rarity.Uncommon:
                        uncommonUnitItems.Add(itemInfo);
                        break;
                    case Rarity.Rare:
                        rareUnitItems.Add(itemInfo);
                        break;
                    case Rarity.Epic:
                        epicUnitItems.Add(itemInfo);
                        break;
                }
            }
            if (itemInfo.Requirement == ItemRequirement.None || itemInfo.Requirement == ItemRequirement.Spell)
            {
                switch (itemInfo.ItemRarity)
                {
                    case Rarity.Common:
                        commonSpellItems.Add(itemInfo);
                        break;
                    case Rarity.Uncommon:
                        uncommonSpellItems.Add(itemInfo);
                        break;
                    case Rarity.Rare:
                        rareSpellItems.Add(itemInfo);
                        break;
                    case Rarity.Epic:
                        epicSpellItems.Add(itemInfo);
                        break;
                }
            }


        }

    }

    public DeckEntry GetNextPurchaseableCard()
    {
        DeckEntry deckEntry = new DeckEntry();
        CardInfo cardInfo = collectibleCards[Random.Range(0, collectibleCards.Count)];
        deckEntry.cardName = cardInfo.name;
        deckEntry.numCopies = 1;

        ItemInfo itemInfo = GetRandomItem(cardInfo);
        if (itemInfo != null)
        {
            deckEntry.items.Add(itemInfo.name);
        }

        return deckEntry;
    }

    public ItemInfo GetRandomItem(string cardTitle)
    {
        CardInfo cardInfo = null;
        cardInfo = cardDictionary.GetByKey(cardTitle);

        if (!cardInfo)
        {
            Debug.LogError($"Card {cardTitle} not found in card dictionary.");
            return null;
        }

        return GetRandomItem(cardInfo);
    }

    public ItemInfo GetRandomItem(CardInfo cardInfo)
    {
        List<ItemInfo> commonItems = null;
        List<ItemInfo> uncommonItems = null;
        List<ItemInfo> rareItems = null;
        List<ItemInfo> epicItems = null;

        if (cardInfo.Type == CardType.Unit)
        {
            commonItems = commonUnitItems;
            uncommonItems = uncommonUnitItems;
            rareItems = rareUnitItems;
            epicItems = epicUnitItems;
        }
        else if (cardInfo.Type == CardType.Spell)
        {
            commonItems = commonSpellItems;
            uncommonItems = uncommonSpellItems;
            rareItems = rareSpellItems;
            epicItems = epicSpellItems;
        }

        float random = Random.Range(0f, 1f);

        if (random < 0.5f)
        {
            return commonItems[Random.Range(0, commonItems.Count)];
        }
        else if (random < 0.8f)
        {
            return uncommonItems[Random.Range(0, uncommonItems.Count)];
        }
        else if (random < 0.95f)
        {
            return rareItems[Random.Range(0, rareItems.Count)];
        }
        else
        {
            return epicItems[Random.Range(0, epicItems.Count)];
        }
    }

    public int CalculateCardPrice(DeckEntry deckEntry)
    {
        return baseCardPrice + deckEntry.items.Count * baseCommonItemPrice;
    }


}
