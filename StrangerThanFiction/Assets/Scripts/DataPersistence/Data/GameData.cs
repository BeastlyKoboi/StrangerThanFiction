using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;

    [Header("Stats")]
    public int totalRunsCount;

    [Header("Story")]
    public List<string> storyParagraphs;

    [Header("Decks")]
    public DeckInventory player1Deck;
    public DeckInventory player2Deck;
    public DeckInventory testDeck;

    public BattleNodeData nextBattleNode;

    [Header("Binding")]
    public int bindingPower;

    public GameData()
    {
        this.totalRunsCount = 0;
        this.storyParagraphs = new List<string>();
        this.player1Deck = new DeckInventory();
        this.player2Deck = new DeckInventory();
        this.testDeck = new DeckInventory();
        this.bindingPower = 20;
    }



}
