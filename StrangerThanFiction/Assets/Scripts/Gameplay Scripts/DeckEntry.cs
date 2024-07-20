using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckEntry
{
    public string cardName;
    public int numCopies;
    public List<string> items; 

    public DeckEntry()
    {
        cardName = "";
        numCopies = 1;
        items = new List<string>();
    }
}
