using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    public UniTaskEvent<RoundStartState> OnRoundStart = new UniTaskEvent<RoundStartState>();
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

        player1Deck = data.GetRunData().player1Deck;

        currentBattleNodeData = battleNodeDictionary.GetByKey(data.GetRunData().nextBattleNode) as BattleNodeData;

        player2Deck = currentBattleNodeData.DeckInventory;
        player2BoonsList.AddRange(currentBattleNodeData.Boons);
    }

    public void SaveData(GameData data)
    {
        data.GetRunData().player1Deck = player1Deck;

        data.GetRunData().combatResults = new CombatResults()
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

        await OnRoundStart.InvokeAsync(new RoundStartState() { 
            currentRoundNumber = roundNumber, 
            maxRounds = maxRounds 
        });

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

        await PromptSwapAlliedUnit();

        await PromptContestedUnits();

        await OnRoundEnd.InvokeAsync();

        uiManager.UpdateTotalPower();

        ResetRoundStats();

        unusedInk += player1.CurrentMana;

        await ResolveCombat();

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

    private async UniTask PromptSwapAlliedUnit()
    {
        if (boardManager.GetUnits(player1).Length == 0) return;

        bool skipSwap = false;
        UnitSlot firstSlot = null;
        UnitSlot secondSlot = null;
        CardModel[] allies = boardManager.GetUnits(player1);
        UnitSlot[] unitSlots = boardManager.playerRow.GetUnitSlots();

        async UniTask<UnitSlot> SelectFirstSlot()
        {
            UnitSlot selectedSlot = null;

            uiManager.SetPrompt(isActive: true, text: "Exert your autonomy. Choose an ally to move, or skip to hold your formation.");
            uiManager.SetRightMiddleButton(visible: true, text: "Skip", () => skipSwap = true);

            await boardManager.SetOnLeftClickForUnits((selectable) => 
            {
                if (selectable is not CardModel unit) return;

                for (int i = 0; i < unitSlots.Length; i++)
                {
                    if (unitSlots[i].Unit == unit) 
                    { 
                        selectedSlot = unitSlots[i];
                        continue;
                    }
                }
                return;
            }, player1);

            do
            {
                await UniTask.Yield();
            } while (selectedSlot == null && !skipSwap);

            await boardManager.SetOnLeftClickForUnits(null, player1);

            if (skipSwap)
                return null;

            return selectedSlot;
        }

        async UniTask<UnitSlot> SelectSecondSlot()
        {
            UnitSlot selectedSlot = null;
            bool cancelSelection = false;

            uiManager.SetPrompt(isActive: true, text: "Choose a new slot or ally to swap positions with.");
            uiManager.SetRightMiddleButton(visible: true, text: "Cancel", () => cancelSelection = true);
            for (int i = 0; i < unitSlots.Length; i++)
            {
                if (!unitSlots[i].IsEmpty()) continue;

                unitSlots[i].SetOnLeftClick((selectable) =>
                {
                    if (selectable is not UnitSlot unitSlot || unitSlot == firstSlot) return;

                    selectedSlot = unitSlot;
                });
            }

            await boardManager.SetOnLeftClickForUnits((selectable) =>
            {
                if (selectable is not CardModel unit) return;

                for (int i = 0; i < unitSlots.Length; i++)
                {
                    if (unitSlots[i].Unit == unit)
                    {
                        selectedSlot = unitSlots[i];
                        continue;
                    }
                }
                return;

            }, player1);

            do
            {
                await UniTask.Yield();

            } while (selectedSlot == null && !cancelSelection);

            await boardManager.SetOnLeftClickForUnits(null, player1);

            if (cancelSelection)
                return null;

            return selectedSlot;
        }


        bool hasCanceledButNotSkipped = false;
        do
        {
            hasCanceledButNotSkipped = false;

            firstSlot = await SelectFirstSlot();

            if (firstSlot != null)
            {
                secondSlot = await SelectSecondSlot();

                if (secondSlot == null)
                    hasCanceledButNotSkipped = true;
            }

        } while (hasCanceledButNotSkipped);

        uiManager.SetPrompt(isActive: false, text: "");
        uiManager.SetRightMiddleButton(visible: false, text: "", null);

        for (int i = 0; i < unitSlots.Length; i++)
        {
            unitSlots[i].SetOnLeftClick(null);
        }

        if (!firstSlot)
            return;

        // Now do the actual replacing.
        CardModel tempStorage = secondSlot.Unit;

        secondSlot.SetNewUnit(firstSlot.Unit, firstSlot.Unit.cardView);

        if (tempStorage)
            firstSlot.SetNewUnit(tempStorage, tempStorage.cardView);
        else
            firstSlot.RemoveUnit();

    }

    private async UniTask PromptContestedUnits()
    {
        CardModel[] allies = boardManager.GetUnits(player1);
        bool hasConfirmed = false;

        uiManager.SetPrompt(isActive: true, text: "Select which allies will strike the Binding.\nEnemies that contest them will block their attack.");
        uiManager.SetRightMiddleButton(visible: true, text: "Confirm", () => hasConfirmed = true);

        for (int i = 0; i < allies.Length; i++)
        {
            allies[i].SetContestedVisible(isContested: false, isVisible: true);
        }

        await boardManager.SetOnLeftClickForUnits((selectable) => 
        {
            if (selectable is not CardModel unit) return;
            unit.ToggleContested(selectable);
        }, player1);

        do
        {
            await UniTask.Yield();
        } while (!hasConfirmed);

        uiManager.SetPrompt(isActive: false, text: "");
        uiManager.SetRightMiddleButton(visible: false, text: "", null);

        await boardManager.SetOnLeftClickForUnits(null, player1);
    }

    private async UniTask ResolveCombat()
    {
        // Apply damage to binding
        UnitSlot[] alliedSlots = boardManager.playerRow.GetUnitSlots();
        UnitSlot[] enemySlots = boardManager.enemyRow.GetUnitSlots();

        for (int i = 0; i < alliedSlots.Length; i++)
        {
            CardModel ally = alliedSlots[i].Unit;
            CardModel enemy = enemySlots[i].Unit;

            bool allyContested = ally && ally.IsContested;
            bool enemyContested = enemy && enemy.IsContested;

            // Case 1: Both sides contested -> they strike each other only.
            if (allyContested && enemyContested)
            {
                await CardModel.SimultaneousStrike(ally, enemy);
                continue; // Each only strikes once
            }

            // Case 2: Only ally is contested -> strikes Binding
            if (allyContested && !enemyContested)
                await ally.Strike(binding);

            // Case 3: Only enemy is contested -> strikes Binding (heals it)
            if (enemyContested && !allyContested)
                await enemy.Strike(binding);

            //if (alliedSlots[i].Unit != null && alliedSlots[i].Unit.IsContested && 
            //    enemySlots[i].Unit != null && enemySlots[i].Unit.IsContested)
            //{
            //    await CardModel.SimultaneousStrike(alliedSlots[i].Unit, enemySlots[i].Unit);
            //}

            //if (alliedSlots[i].Unit && alliedSlots[i].Unit.IsContested)
            //    await alliedSlots[i].Unit.Strike(binding);

            //if (enemySlots[i].Unit && enemySlots[i].Unit.IsContested)
            //    await enemySlots[i].Unit.Strike(binding);
        }

        for (int i = 0; i < alliedSlots.Length; i++)
        {
            if (alliedSlots[i].Unit)
                alliedSlots[i].Unit.SetContestedVisible(isContested: false, isVisible: false);
        }

        //for (int i = 0; i < alliedSlots.Length; i++)
        //{
        //    if (alliedSlots[i].Unit)
        //        await alliedSlots[i].Unit.Strike(binding);
        //}

        //if (binding.BindingDamage < binding.BindingPower)
        //{
        //    for (int i = 0; i < enemySlots.Length; i++)
        //    {
        //        if (enemySlots[i].Unit)
        //            await enemySlots[i].Unit.Strike(binding);
        //    }
        //}
    }


    private void ResetRoundStats()
    {
        // Reset round stats for both players
        player1.ResetRoundStats();
        player2.ResetRoundStats();
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
