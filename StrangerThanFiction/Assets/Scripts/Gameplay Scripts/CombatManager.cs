using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

/// <summary>
/// Manages the flow of game in a single match. 
/// TODO: Add more events for more specific actions.
/// TODO: Add more functionality for battlefield conditions.
/// TODO: Rework save data 
/// TODO:
/// </summary>
public class CombatManager : MonoBehaviour, IDataPersistence
{
    // Basic gameplay events that objects can add to
    public UniTaskEvent OnGameStart = new UniTaskEvent();
    public UniTaskEvent OnRoundStart = new UniTaskEvent();
    public UniTaskEvent OnRoundEnd = new UniTaskEvent();
    public UniTaskEvent OnGameOver = new UniTaskEvent();

    [HeaderAttribute("The Players")]
    public Player player1; // The human player
    public Player player2; // The AI eventually 

    [HeaderAttribute("Input Actions")]
    [SerializeField] private InputActionReference pause;

    [HeaderAttribute("Managers")]
    public UIManager uiManager;
    public BoardManager boardManager;

    [HeaderAttribute("Game State Information")]
    public Binding binding;
    public int roundNumber = 0;
    public const int maxRounds = 6;
    public int unusedInk = 0;

    [HeaderAttribute("Text Assets")]
    [SerializeField] private bool usingInspector;
    [SerializeField] private DeckInventory player1Deck;
    [SerializeField] private DeckInventory player2Deck;

    // Something for battlefield conditions


    public void LoadData(GameData data)
    {
        if (usingInspector) return;

        // load the binding from the data

        player1Deck = data.player1Deck;
        player2Deck = data.player2Deck;
    }

    public void SaveData(GameData data)
    {
        data.player1Deck = player1Deck;
        data.player2Deck = player2Deck;
    }

    private void Awake()
    {
        CardFactory.Instance.Initialize();

        OnGameStart.Clear();

        OnRoundStart.Clear();
        OnRoundStart.AddListener(player1.RoundStart);
        OnRoundStart.AddListener(player2.RoundStart);
        OnRoundStart.AddListener(boardManager.RoundStart);

        OnRoundEnd.Clear();
        OnRoundEnd.AddListener(player1.RoundEnd);
        OnRoundEnd.AddListener(player2.RoundEnd);
        OnRoundEnd.AddListener(boardManager.RoundEnd);

        OnGameOver.Clear();
    }

    /// <summary>
    /// Method to initialize the game.
    /// </summary>
    void Start()
    { 
        pause.action.performed += ctx => TogglePause();

        // initialize all needed stuff for beginning of game 

        //player1.PopulateDeck(player1Deck.ToArray(), false);
        //player2.PopulateDeck(player2Deck.ToArray(), true);

        player1.PopulateDeck(player1Deck, false);
        player2.PopulateDeck(player2Deck, true);

       
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
        await UniTask.Delay(1000);

        await OnGameStart.InvokeAsync();

        do
        {
            roundNumber++;
            await RoundActivity();

        } while (roundNumber < maxRounds && binding.BindingDamage < binding.BindingPower);

        await EndGame();
    }

    /// <summary>
    /// Method to go over the activity of a single round. 
    /// </summary>
    /// <returns></returns>
    private async UniTask RoundActivity()
    {
        uiManager.RoundStart(roundNumber, maxRounds);

        // Consider reseting mana before round start. 
        player1.ResetMana();
        player2.ResetMana();

        await DrawHands(); // ITF maybe put this in event with numCards to draw as a variable

        await OnRoundStart.InvokeAsync();

        uiManager.UpdateTotalPower();

        // Draw Cards
        do
        {
            await UniTask.Delay(100);

            if (player1.CanDoSomething())
                await player1.PlayerTurn();

            await UniTask.Delay(100);

            if (player2.CanDoSomething())
                await player2.PlayerTurn();

            await UniTask.Yield();

        } while ((player1.CanDoSomething() && !player1.hasEndedTurn) ||
            (player2.CanDoSomething() && !player2.hasEndedTurn));

        await DiscardHands();

        await OnRoundEnd.InvokeAsync();

        uiManager.UpdateTotalPower();

        // Apply damage to binding
        CardModel[] units = boardManager.GetUnits(player1);
        for (int i = 0; i < units.Length; i++)
        {
            await units[i].Strike(binding);
        }

        if (binding.BindingDamage < binding.BindingPower)
        {
            units = boardManager.GetUnits(player2);
            for (int i = 0; i < units.Length; i++)
            {
                await units[i].Strike(binding);
            }
        }

        await CardFactory.Instance.QueueLockedCards();
    }

    /// <summary>
    /// Method to draw hands for both players. Used at round start
    /// </summary>
    /// <param name="numCards"></param>
    /// <returns></returns>
    private async UniTask DrawHands(int numCards = 5)
    {
        for (int i = 0; i < numCards; i++)
        {
            await player1.DrawCard();
            await player2.DrawCard();
            await UniTask.Delay(500);
        }
    }

    /// <summary>
    /// Method to discard the hands of both players. Used at round end.
    /// </summary>
    /// <returns></returns>
    private async UniTask DiscardHands()
    {
        await DiscardPlayerHand(player1);
        await DiscardPlayerHand(player2);
        await UniTask.Delay(2000);
    }

    /// <summary>
    /// Method to discard a player's hand.
    /// </summary>
    /// <param name="player"></param>
    private async UniTask DiscardPlayerHand(Player player)
    {
        foreach (CardModel card in player.handManager.Hand.ToArray())
        {
            await player.DiscardCard(card);
        }
    }

    /// <summary>
    /// Method to end the game.
    /// </summary>
    private async UniTask EndGame()
    {
        uiManager.GameOver(new GameOverState(
            binding.BindingDamage >= binding.BindingPower, 
            unusedInk,
            binding.BindingPower,
            binding.BindingDamage)); // Maybe add this to event
        await OnGameOver.InvokeAsync();
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
