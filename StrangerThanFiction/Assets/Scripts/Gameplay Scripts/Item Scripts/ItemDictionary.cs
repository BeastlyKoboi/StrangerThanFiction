using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDictionary", menuName = "Cards/Item Dictionary")]
public class ItemDictionary : ScriptableObject
{
    public List<ItemEntry> itemEntries = new List<ItemEntry>();
    private Dictionary<string, ItemInfo> itemDictionary;

    [System.Serializable]
    public class ItemEntry
    {
        public string itemName;
        public ItemInfo itemData;
    }

    public void InitializeDictionary()
    {
        itemDictionary = new Dictionary<string, ItemInfo>();
        foreach (var entry in itemEntries)
        {
            if (!itemDictionary.ContainsKey(entry.itemName))
                itemDictionary[entry.itemName] = entry.itemData;

        }
    }

    public ItemInfo GetItemDataByName(string name)
    {
        if (itemDictionary == null)
            InitializeDictionary();
        return itemDictionary.TryGetValue(name, out var cardData) ? cardData : null;
    }

    public ItemInfo GetItemDataByIndex(int index)
    {
        if (itemDictionary == null)
            InitializeDictionary();
        return itemDictionary.TryGetValue(itemEntries[index].itemName, out var cardData) ? cardData : null;
    }
}
