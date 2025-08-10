using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunData
{
    [Header("Decks")]
    public DeckInventory player1Deck;
    public DeckInventory testDeck;

    [Header("Boons")]
    public List<string> boonList;

    public string nextBattleNode;

    [Header("Binding")]
    public int bindingPower;

    [Header("Nodemap")]
    public FlatNodeMap nodeMap;

    [Header("Combat Results")]
    public CombatResults combatResults;

    [Header("Current Run Stats")]
    public string currentSeed;
    public Random.State randomState;
    public int currency;
    public int rerollTokens;

    public bool runHasStarted;
    public bool runHasEnded;

    public RunData()
    {
        this.player1Deck = new DeckInventory();
        this.testDeck = new DeckInventory();

        this.boonList = new List<string>();

        this.bindingPower = 20;

        this.nodeMap = new FlatNodeMap();

        this.combatResults = new CombatResults();

        this.currency = 25;
        this.rerollTokens = 3;

        this.runHasStarted = false;
        this.runHasEnded = false;
    }


}
