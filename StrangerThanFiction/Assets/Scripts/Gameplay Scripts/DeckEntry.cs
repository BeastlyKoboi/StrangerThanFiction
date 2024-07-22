using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckEntry
{
    public string cardName;
    public int numCopies;
    public List<string> items; 

    public DeckEntry(string cardName = "", int numCopies = 1)
    {
        this.cardName = cardName;
        this.numCopies = numCopies;
        items = new List<string>();
    }
}
