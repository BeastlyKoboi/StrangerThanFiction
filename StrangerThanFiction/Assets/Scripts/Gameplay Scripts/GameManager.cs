using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Manages the flow of game in a single match. 
/// TODO: Add more events for more specific actions.
/// TODO: Add more functionality for battlefield conditions.
/// TODO: Rework save data 
/// TODO:
/// </summary>
public class GameManager : MonoBehaviour, IDataPersistence
{
    // Basic gameplay events that objects can add to
    public static event Func<Task> OnGameStart;
    public static event Func<Task> OnRoundStart;
    public static event Func<Task> OnRoundEnd;
    public static event Func<Task> OnGameOver;

    [HeaderAttribute("The Players")]
    public Player player1; // The human player
    public Player player2; // The AI eventually 

    [HeaderAttribute("Input Actions")]
    [SerializeField] private InputActionReference pause;

    [HeaderAttribute("Managers")]
    public UIManager uiManager;
    public BoardManager boardManager;

    [HeaderAttribute("Game State Information")]
    public int roundNumber = 0;
    public const int totalRounds = 6;

    [HeaderAttribute("Card Prefabs")]
    public GameObject cardPrefab;
    public GameObject unitPrefab;

    [HeaderAttribute("Text Assets")]
    [SerializeField] private bool usingInspector;
    [SerializeField] private DeckInventory player1Deck;
    [SerializeField] private DeckInventory player2Deck;

    // Something for battlefield conditions
    //  - 


    public void LoadData(GameData data)
    {
        if (usingInspector) return;

        player1Deck = data.player1Deck;
        player2Deck = data.player2Deck;
    }

    public void SaveData(GameData data)
    {
        data.player1Deck = player1Deck;
        data.player2Deck = player2Deck;
    }

    /// <summary>
    /// Method to initialize the game.
    /// </summary>
    void Start()
    {
        pause.action.performed += ctx => TogglePause();

        // initialize all needed stuff for beginning of game 


        CardFactory.Instance.Initialize(cardPrefab, unitPrefab);

        //player1.PopulateDeck(player1Deck.ToArray(), false);
        //player2.PopulateDeck(player2Deck.ToArray(), true);

        player1.PopulateDeck(player1Deck, false);
        player2.PopulateDeck(player2Deck, true);

        OnGameStart = null;

        OnRoundStart = null;
        OnRoundStart += player1.RoundStart;
        OnRoundStart += player2.RoundStart;
        OnRoundStart += boardManager.RoundStart;

        OnRoundEnd = null;
        OnRoundEnd += player1.RoundEnd;
        OnRoundEnd += player2.RoundEnd;
        OnRoundEnd += boardManager.RoundEnd;

        OnGameOver = null;

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

        await EndGame();
    }

    /// <summary>
    /// Method to go over the activity of a single round. 
    /// </summary>
    /// <returns></returns>
    private async Task RoundActivity()
    {
        uiManager.RoundStart(roundNumber);

        if (OnRoundStart != null)
        {
            foreach (Func<Task> handler in OnRoundStart.GetInvocationList()
                .Cast<Func<Task>>().ToList())
            {
                await handler();
            }
        }

        // Consider reseting mana before round start. 
        player1.ResetMana();
        player2.ResetMana();

        await DrawHands(); // ITF maybe put this in event with numCards to draw as a variable

        // Draw Cards
        do
        {
            await Task.Delay(100);

            if (player1.CanDoSomething())
                await player1.PlayerTurn();

            await Task.Delay(100);

            if (player2.CanDoSomething())
                await player2.PlayerTurn();

            await Task.Yield();

        } while ((player1.CanDoSomething() && !player1.hasEndedTurn) ||
            (player2.CanDoSomething() && !player2.hasEndedTurn));
        // 

        await DiscardHands();

        if (OnRoundEnd != null)
        {
            foreach (Func<Task> handler in OnRoundEnd.GetInvocationList()
                .Cast<Func<Task>>().ToList())
            {
                await handler();
            }
        }
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
            await player1.DrawCard();
            await player2.DrawCard();
            await Task.Delay(500);
        }
    }

    /// <summary>
    /// Method to discard the hands of both players. Used at round end.
    /// </summary>
    /// <returns></returns>
    private async Task DiscardHands()
    {
        await DiscardPlayerHand(player1);
        await DiscardPlayerHand(player2);
        await Task.Delay(2000);
    }

    /// <summary>
    /// Method to discard a player's hand.
    /// </summary>
    /// <param name="player"></param>
    private async Task DiscardPlayerHand(Player player)
    {
        foreach (CardModel card in player.handManager.Hand.ToArray())
        {
            await player.DiscardCard(card);
        }
    }

    /// <summary>
    /// Method to end the game.
    /// </summary>
    private async Task EndGame()
    {
        uiManager.GameOver(); // Maybe add this to event
        if (OnGameOver != null)
        {
            foreach (Func<Task> handler in OnGameOver.GetInvocationList()
                .Cast<Func<Task>>().ToList())
            {
                await handler();
            }
        }
    }

    private void TogglePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            uiManager.TogglePausedMenu(true);
        }
        else
        {
            Time.timeScale = 1;
            uiManager.TogglePausedMenu(false);
        }
    }

    /// <summary>
    /// Method to quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
