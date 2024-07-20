using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckInventory
{
    public List<DeckEntry> deckEntries;

    public DeckInventory()
    {
        deckEntries = new List<DeckEntry>();
    }
}
