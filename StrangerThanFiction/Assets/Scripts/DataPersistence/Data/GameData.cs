using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    public int totalRunsCount;

    public DeckInventory player1Deck;
    public DeckInventory player2Deck;
    public DeckInventory testDeck;

    public GameData()
    {
        this.totalRunsCount = 0;
        this.player1Deck = new DeckInventory();
        this.player2Deck = new DeckInventory();
        this.testDeck = new DeckInventory();
    }



}
