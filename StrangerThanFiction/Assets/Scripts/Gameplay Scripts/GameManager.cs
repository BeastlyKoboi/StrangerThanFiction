using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the flow of game in a single match. 
/// TODO: Add more events for more specific actions.
/// TODO: Add more functionality for battlefield conditions.
/// TODO: Rework save data 
/// TODO:
/// </summary>
public class GameManager : MonoBehaviour
{
    // Basic gameplay events that objects can add to
    public static event Action OnGameStart;
    public static event Action OnRoundStart;
    public static event Action OnRoundEnd;
    public static event Action OnGameOver;

    [HeaderAttribute("The Players")]
    public Player player1; // The human player
    public Player player2; // The AI eventually 

    [HeaderAttribute("Managers")]
    public UIManager uiManager;
    public BoardManager boardManager;

    [HeaderAttribute("Game State Information")]
    public int roundNumber = 0;
    public const int totalRounds = 6;

    [HeaderAttribute("Text Assets")]
    [SerializeField] private TextAsset StarterDecksJSON;
    private string[] Pinocchio = new string[]
    {
        "Donkey",
        "GrowthSpurt",
        "Donkey",
        "GrowthSpurt",
        "Donkey",
        "GrowthSpurt",
        "TheBlueFairy",
        "GrowthSpurt",
        "Donkey",
        "GrowthSpurt",
        "TheBlueFairy",
        "GrowthSpurt",
        "Donkey",
        "GrowthSpurt",
        "Donkey",
        "TheBlueFairy",
        "Pinocchio",
        "GrowthSpurt"
    };
    private string[] TheBigBadWolf;

    // Something for battlefield conditions
    //  - 


    /// <summary>
    /// Method to initialize the game.
    /// </summary>
    void Start()
    {
        // initialize all needed stuff for beginning of game 

        JsonUtility.FromJsonOverwrite(StarterDecksJSON.text, this);

        //gameObject.AddComponent<LocalStorageManager>();
        //LocalStorageManager storage = gameObject.GetComponent<LocalStorageManager>();
        //storage.SaveData("Starter Deck", StarterDecksJSON.text);

        player1.PopulateDeck(Pinocchio, false);
        player2.PopulateDeck(Pinocchio, true);

        OnRoundStart += player1.RoundStart;
        OnRoundStart += player2.RoundStart;

        OnRoundStart += boardManager.RoundStart;

        // Call on game start 
        StartGame();

    }

    /// <summary>
    /// Method to trigger the game loop.
    /// </summary>
    private void StartGame()
    {
        // Setup 


        GameLoop();
    }

    /// <summary>
    /// Method to proceed through the rounds, and confirm a winner. 
    /// </summary>
    private async void GameLoop()
    {
        await Task.Delay(1000);

        do
        {
            roundNumber++;
            await RoundActivity();

        } while (roundNumber < totalRounds);

        // Decide who has most power then grant win to player

        EndGame();
    }

    /// <summary>
    /// Method to go over the activity of a single round. 
    /// </summary>
    /// <returns></returns>
    private async Task RoundActivity()
    {
        uiManager.RoundStart(roundNumber);

        OnRoundStart?.Invoke();

        // Consider reseting mana before round start. 
        player1.ResetMana();
        player2.ResetMana();

        await DrawHands(); // ITF maybe put this in event with numCards to draw as a variable

        // Draw Cards
        do
        {
            await Task.Delay(1000);

            if (player1.CanDoSomething())
                await player1.PlayerTurn();

            await Task.Delay(1000);

            if (player2.CanDoSomething())
                await player2.PlayerTurn();

            await Task.Yield();

        } while ((player1.CanDoSomething() && !player1.hasEndedTurn) ||
            (player2.CanDoSomething() && !player2.hasEndedTurn));
        // 

        await DiscardHands();

        OnRoundEnd?.Invoke();
    }

    /// <summary>
    /// Method to draw hands for both players. Used at round start
    /// </summary>
    /// <param name="numCards"></param>
    /// <returns></returns>
    private async Task DrawHands(int numCards = 5)
    {
        for (int i = 0; i < numCards; i++)
        {
            player1.DrawCard();
            player2.DrawCard();
            await Task.Delay(500);
        }
    }

    /// <summary>
    /// Method to discard the hands of both players. Used at round end.
    /// </summary>
    /// <returns></returns>
    private async Task DiscardHands()
    {
        DiscardPlayerHand(player1);
        DiscardPlayerHand(player2);
        await Task.Delay(2000);
    }

    /// <summary>
    /// Method to discard a player's hand.
    /// </summary>
    /// <param name="player"></param>
    private void DiscardPlayerHand(Player player)
    {
        foreach (CardModel card in player.handManager.Hand.ToArray())
        {
            player.DiscardCard(card);
        }
    }

    /// <summary>
    /// Method to end the game.
    /// </summary>
    private void EndGame()
    {
        uiManager.GameOver(); // Maybe add this to event
        OnGameOver?.Invoke();
    }

    /// <summary>
    /// Method to quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}