using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.SceneManagement;

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
    public UniTaskEvent<CombatEnterState> OnGameStart = new UniTaskEvent<CombatEnterState>();
    public UniTaskEvent OnRoundStart = new UniTaskEvent();
    public UniTaskEvent OnRoundEnd = new UniTaskEvent();
    public UniTaskEvent OnGameOver = new UniTaskEvent();

    [HeaderAttribute("The Players")]
    public Player player1; // The human player
    public Player player2; // The AI eventually 

    [HeaderAttribute("Managers")]
    public UIManager uiManager;
    public BoardManager boardManager;
    public RunManager runManager;

    [HeaderAttribute("Game State Information")]
    public Binding binding;
    public int roundNumber = 0;
    public const int maxRounds = 6;
    public int unusedInk = 0;

    [HeaderAttribute("Text Assets")]
    [SerializeField] private bool usingInspector;
    [SerializeField] private BattleNodeDictionary battleNodeDictionary;
    [SerializeField] private BattleNodeData currentBattleNodeData;
    [SerializeField] private DeckInventory player1Deck;
    [SerializeField] private DeckInventory player2Deck;
    [SerializeField] private List<string> player2BoonsList = new List<string>();

    private CancellationTokenSource cts;


    public void LoadData(GameData data)
    {
        if (usingInspector) return;

        // load the binding from the data
        Debug.Log("Loading data in combat manager");

        player1Deck = data.player1Deck;

        currentBattleNodeData = battleNodeDictionary.GetByKey(data.nextBattleNode) as BattleNodeData;

        player2Deck = currentBattleNodeData.DeckInventory;
        player2BoonsList.AddRange(currentBattleNodeData.Boons);
    }

    public void SaveData(GameData data)
    {
        data.player1Deck = player1Deck;

        data.combatResults = new CombatResults()
        {
            victory = binding.BindingDamage >= binding.BindingPower,
            spentInk = 0,
            gainedDeus = unusedInk
        };
    }

    private void Awake()
    {
        cts = new CancellationTokenSource();
        SceneManager.activeSceneChanged += OnSceneChanged;

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
        // initialize all needed stuff for beginning of game 

        //player1.PopulateDeck(player1Deck.ToArray(), false);
        //player2.PopulateDeck(player2Deck.ToArray(), true);

        player1.PopulateDeck(player1Deck, false);
        player2.PopulateDeck(player2Deck, true);

        player1.BoonCollection = runManager.BoonCollection;
        player2.ApplyBoons(player2BoonsList);

        OnGameStart.AddListener( async (CombatEnterState combatEnterState) =>
        {
            player1.BoonCollection.EnterCombat(player1);
            player2.BoonCollection.EnterCombat(player2);
        });

        OnGameOver.AddListener(async () =>
        {
            player1.BoonCollection.ExitCombat();
            player2.BoonCollection.ExitCombat();
        });



        // Call on game start 
        StartGame();

    }

    /// <summary>
    /// Method to trigger the game loop.
    /// </summary>
    private void StartGame()
    {
        // Setup 
        try
        {
            GameLoop();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Task was cancelled");
        }
    }


    /// <summary>
    /// Method to proceed through the rounds, and confirm a winner. 
    /// </summary>
    private async void GameLoop()
    {
        await UniTask.Delay(1000, cancellationToken: cts.Token);

        await OnGameStart.InvokeAsync(new CombatEnterState());

        do
        {
            roundNumber++;
            try
            {
                await RoundActivity();

            }
            catch (OperationCanceledException)
            {
                Debug.Log("Round Activity was cancelled");

            }

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
            await UniTask.Delay(100, cancellationToken: cts.Token);

            if (player1.CanDoSomething())
                await player1.PlayerTurn();

            await UniTask.Delay(100, cancellationToken: cts.Token);

            if (player2.CanDoSomething())
                await player2.PlayerTurn();

            await UniTask.Yield();

        } while ((player1.CanDoSomething() && !player1.hasEndedTurn) ||
            (player2.CanDoSomething() && !player2.hasEndedTurn));

        await DiscardHands();

        await OnRoundEnd.InvokeAsync();

        uiManager.UpdateTotalPower();

        unusedInk += player1.CurrentMana;

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
            await UniTask.Delay(500, cancellationToken: cts.Token);
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
        await UniTask.Delay(2000, cancellationToken: cts.Token);
    }

    /// <summary>
    /// Method to discard a player's hand.
    /// </summary>
    /// <param name="player"></param>
    private async UniTask DiscardPlayerHand(Player player)
    {
        foreach (CardModel card in player.handManager.Hand.ToArray())
        {
            if (!card.HasCondition("Keep"))
                await player.DiscardCard(card);
        }
    }

    /// <summary>
    /// Method to end the game.
    /// </summary>
    private async UniTask EndGame()
    {
        unusedInk += (maxRounds - roundNumber) * player1.MaxMana;

        uiManager.GameOver(new GameOverState(
            binding.BindingDamage >= binding.BindingPower, 
            unusedInk,
            binding.BindingPower,
            binding.BindingDamage)); // Maybe add this to event
        await OnGameOver.InvokeAsync();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        cts?.Cancel(); // Cancel any running tasks when the scene changes
        cts?.Dispose();
        cts = new CancellationTokenSource(); // Reset for new tasks
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        cts?.Cancel();
        cts?.Dispose();
    }

}
