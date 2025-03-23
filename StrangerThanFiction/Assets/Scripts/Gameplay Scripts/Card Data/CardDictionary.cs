using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDictionary", menuName = "Cards/Card Dictionary")]
public class CardDictionary : ScriptableObject
{
    public List<CardEntry> cardEntries = new List<CardEntry>();
    private Dictionary<string, CardInfo> cardDictionary;

    [System.Serializable]
    public class CardEntry
    {
        public string cardName;
        public CardInfo cardData;
    }

    public void InitializeDictionary()
    {
        cardDictionary = new Dictionary<string, CardInfo>();
        foreach (var entry in cardEntries)
        {
            if (!cardDictionary.ContainsKey(entry.cardName))
                cardDictionary[entry.cardName] = entry.cardData;

        }
    }

    public CardInfo GetCardDataByName(string name)
    {
        if (cardDictionary == null)
            InitializeDictionary();
        return cardDictionary.TryGetValue(name, out var cardData) ? cardData : null;
    }

    public CardInfo GetCardDataByIndex(int index)
    {
        if (cardDictionary == null)
            InitializeDictionary();
        return cardDictionary.TryGetValue(cardEntries[index].cardName, out var cardData) ? cardData : null;
    }
}
