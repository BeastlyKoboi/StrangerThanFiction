using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.UI.CanvasScaler;

/// <summary>
/// My thinking is that all player input will be filtered in from here, human or AI. 
/// The player's UI will have access to their player and the AI will have access 
/// to their players methods behind the scenes. Upon player activity the game manager 
/// will prompt and manage the actions (somehow). 
/// </summary>

public class Player : MonoBehaviour
{
    public UniTaskEvent OnGameStart = new UniTaskEvent();
    public UniTaskEvent<RoundStartState> OnRoundStart = new UniTaskEvent<RoundStartState>();
    public UniTaskEvent OnRoundEnd = new UniTaskEvent();
    public UniTaskEvent OnGameOver = new UniTaskEvent();

    public UniTaskEvent<CardModel> OnCardDrawn = new UniTaskEvent<CardModel>();
    public UniTaskEvent<CardModel> OnBeforeUnitSummoned = new UniTaskEvent<CardModel>();
    public UniTaskEvent<CardModel> OnAfterUnitSummoned = new UniTaskEvent<CardModel>();
    public UniTaskEvent<DamageData> OnAfterUnitSurvivedDamage = new UniTaskEvent<DamageData>();
    public UniTaskEvent<Condition> OnAfterUnitConditionApplied = new UniTaskEvent<Condition>();
    public UniTaskEvent<CardModel> OnUnitDestroyed = new UniTaskEvent<CardModel>();
    public UniTaskEvent<CardPlayState> OnBeforeCardPlayed = new UniTaskEvent<CardPlayState>();
    public UniTaskEvent<CardPlayState> OnAfterCardPlayed = new UniTaskEvent<CardPlayState>();
    public event Action OnMyTurnStart;

    [HeaderAttribute("Game and Enemy Info")]
    public CombatManager combatManager;
    public UIManager uiManager;
    public BoardManager board;
    public Player enemyPlayer;
    public BoonCollection BoonCollection { get; set; } = new BoonCollection();


    private int _maxMana = 5;
    public int MaxMana
    {
        get { return _maxMana; }
        set
        {
            _maxMana = value;
            uiManager.UpdateMana(this);
        }
    }
    private int _currentMana = 5;
    public int CurrentMana
    {
        get { return _currentMana; }
        set
        {
            _currentMana = value;
            uiManager.UpdateMana(this);
        }
    }

    [HeaderAttribute("Game State Info")]
    private bool hasCardsHidden;
    public bool hasEndedTurn = false;
    private bool hasCanceledPlayCard = false;
    public int NumUnitsHealedThisRound { get; set; } = 0;
    public int NumUnitsHealedThisCombat { get; set; } = 0;
    public int NumUnitsRevivedThisCombat { get; set; } = 0;
    public int NumUnitsSurvivedDamageThisRound { get; set; } = 0;
    public int NumUnitsSurvivedDamageThisCombat { get; set; } = 0;


    [HeaderAttribute("The Cards")]
    public HandManager handManager;
    public CardPile Deck { get; private set; }
    public GameObject deckGameObject;
    public GameObject deckViewParent;

    public CardPile Discard { get; private set; }
    public GameObject discardGameObject;
    public GameObject discardViewParent;


    [HeaderAttribute("Card Prefabs")]
    public GameObject cardPrefab;
    public GameObject unitPrefab;

    /// <summary>
    /// Method to initialize the player
    /// </summary>
    void Start()
    {
        OnRoundStart.AddListener(handManager.RoundStart);
        OnRoundEnd.AddListener(handManager.RoundEnd);
    }

    public void PopulateDeck(DeckInventory deckInventory, bool isHidden)
    {
        CardFactory.Instance.RegisterDeck(this, deckInventory);

        hasCardsHidden = isHidden;

        Deck = new CardPile();

        Deck.OnChange += () =>
        {
            uiManager.UpdateDeck(this);
        };
        Deck.OnCardAdded += (card) =>
        {
            if (deckViewParent == null) return;
            card.transform.SetParent(deckViewParent.transform);
        };
        Deck.OnCardRemoved += (card) =>
        {
            if (deckViewParent == null) return;
            card.transform.SetParent(deckGameObject.transform);
            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.anchoredPosition = Vector2.zero;
            cardRect.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            cardRect.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        };

        foreach (DeckEntry entry in deckInventory.GetDeckEntries())
        {
            for (int i = 0; i < entry.numCopies; i++)
            {
                Deck.Add(CardFactory.Instance.CreateCard(entry.cardName, isHidden, deckGameObject.transform, this, board));
            }
        }

        Deck.Shuffle();

        Discard = new CardPile();
        Discard.OnChange += () =>
        {
            uiManager.UpdateDiscard(this);
        };

        Discard.OnCardAdded += (card) =>
        {
            if (discardViewParent == null) return;
            card.transform.SetParent(discardViewParent.transform);
        };
        Discard.OnCardRemoved += (card) =>
        {
            if (discardViewParent == null) return;
            card.transform.SetParent(discardViewParent.transform);
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        };
    }

    public void ApplyBoons(List<string> boonNames)
    {
        foreach (string boonName in boonNames)
        {
            Type boonType = Type.GetType(boonName);
            Boon boon = (Boon)Activator.CreateInstance(boonType);
            BoonCollection.AddBoon(boon);
        }
    }

    public CardModel CreateCardInDeck(string cardName)
    {
        CardModel card = CardFactory.Instance.CreateCard(cardName, hasCardsHidden, deckGameObject.transform, this, board);
        Deck.Add(card);
        Deck.Shuffle();
        return card;
    }

    public CardModel CreateCardInDiscard(string cardName)
    {
        CardModel card = CardFactory.Instance.CreateCard(cardName, hasCardsHidden, discardGameObject.transform, this, board);
        Discard.Add(card);
        Discard.Shuffle();
        return card;
    }

    public CardModel CreateCardInHand(string cardName)
    {
        CardModel card = CardFactory.Instance.CreateCard(cardName, hasCardsHidden, deckGameObject.transform, this, board);
        handManager.AddCardToHandFromDeck(card);
        RefreshPlayableCards();
        return card;
    }

    public void MoveCardFromHandToDeck(CardModel card, bool moveToTop = false, bool shuffleAfter = true)
    {
        handManager.RemoveCardFromHand(card);

        int deckIndex = moveToTop? Deck.Count: 0;
        Deck.Insert(deckIndex, card); 

        if (shuffleAfter) 
            Deck.Shuffle();

        RefreshPlayableCards();
    }

    /// <summary>
    /// This will eventually be called every time an action is 
    /// taken that can change whether a card is playable. It should 
    /// check each card's play requirements and make sure that they are 
    /// met, and if not disable their draggable component. 
    /// </summary>
    public void RefreshPlayableCards()
    {
        handManager.NumPlayableCards = 0;

        handManager.Hand.ForEach((card) =>
        {
            bool isPlayable = true;
            
            if (card.CurrentCost > card.Owner.CurrentMana)
                isPlayable = false;

            if (card.PlayRequirements.AllyUnitTargets > board.GetUnits(this).Length)
                isPlayable = false;
            if (card.PlayRequirements.EnemyUnitTargets > board.GetUnits(enemyPlayer).Length)
                isPlayable = false;
            if (card.PlayRequirements.AllyCardTargets > handManager.Hand.Count - 1)
                isPlayable = false;
            if (card.PlayRequirements.EnemyCardTargets > enemyPlayer.handManager.Hand.Count)
                isPlayable = false;
            if (card.PlayRequirements.AllyHandSize > handManager.Hand.Count)
                isPlayable = false;
            if (card.PlayRequirements.EnemyHandSize > enemyPlayer.handManager.Hand.Count)
                isPlayable = false;

            card.Playable = isPlayable;
            if (isPlayable) handManager.NumPlayableCards++;
        });
    }

    /// <summary>
    /// Method to handle the player's turn. 
    /// </summary>
    /// <returns></returns>
    public async UniTask PlayerTurn()
    {
        hasEndedTurn = false;
        bool playedSuccessfully = false;

        if (combatManager.player1 == this)
            uiManager.SetRightMiddleButton("End Turn", PassTurn);

        RefreshPlayableCards();

        OnMyTurnStart?.Invoke();

        do
        {
            while (handManager.PlayState == null && !hasEndedTurn)
            {
                await UniTask.Yield();
            }

            if (handManager.PlayState != null)
            {
                playedSuccessfully = await PlayCard(handManager.PlayState);
                if (!playedSuccessfully)
                {
                    handManager.SetCardPlayState(null);
                }
                uiManager.UpdateTotalPower();
            }

        } while (!playedSuccessfully && !hasEndedTurn);

        if (combatManager.player1 == this)
            uiManager.SetRightMiddleButton("", () => { });

        handManager.LockCards();
    }

    /// <summary>
    /// Method to draw a card from the player's deck.
    /// </summary>
    public async UniTask DrawCard(CardModel specificCard = null)
    {
        if (Deck.Count == 0) ShuffleDiscardIntoDeck();
        if (Deck.Count == 0) return;

        CardModel drawnCard;

        if (specificCard != null && Deck.ToArray().Contains(specificCard))
        {
            Deck.Remove(specificCard);
            drawnCard = specificCard;
        }
        else
        {
            drawnCard = Deck[Deck.Count - 1];
            Deck.RemoveAt(Deck.Count - 1);
        }

        await OnCardDrawn.InvokeAsync(drawnCard);

        handManager.AddCardToHandFromDeck(drawnCard);
        RefreshPlayableCards();
    }

    /// <summary>
    /// Method to discard a card from the player's hand.
    /// </summary>
    /// <param name="card"></param>
    public async UniTask DiscardCard(CardModel card)
    {
        handManager.RemoveCardFromHand(card);
        RefreshPlayableCards();

        card.gameObject.transform.SetParent(discardGameObject.transform, true);
        await card.Discard(this);

        Discard.Add(card);
    }

    /// <summary>
    /// Method to destroy a card.
    /// </summary>
    /// <param name="card"></param>
    public async UniTask DestroyCard(CardModel card)
    {
        handManager.RemoveCardFromHand(card);
        RefreshPlayableCards();

        await card.Destroy();
    }

    /// <summary>
    /// Method to pass the player's turn.
    /// </summary>
    public void PassTurn() => hasEndedTurn = true;

    /// <summary>
    /// Method to check if the player can play any cards.
    /// </summary>
    public bool CanDoSomething()
    {
        RefreshPlayableCards();
        return handManager.NumPlayableCards > 0;
    }

    /// <summary>
    /// Method to reset the player's mana.
    /// </summary>
    public void ResetMana() => CurrentMana = MaxMana;

    public void ResetRoundStats()
    {
        NumUnitsHealedThisRound = 0;
        NumUnitsSurvivedDamageThisRound = 0;
    }

    /// <summary>
    /// Method to play a card.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private async UniTask<bool> PlayCard(CardPlayState playState)
    {
        // Check if the player has the requirements to play the card.
        PlayRequirements playReqs = playState.card.PlayRequirements;

        if (combatManager.player1 == this)
        {
            uiManager.SetRightMiddleButton("Cancel", CancelPlay);

            CardModel clickedCard = null;
            void onCardClicked(CardModel cardModel)
            {
                clickedCard = cardModel;
            }

            async UniTask getTargetsFromBoard(Player player, List<CardModel> targetList, int playReq)
            {
                uiManager.SetPrompt(true, $"Select {playReq} " +
                    $"{(combatManager.player1 == player? "allied" : "enemy")} " +
                    $"unit{(playReq > 1? "s": "")}");
                await board.SetOnClickForPlayersUnits(player, onCardClicked);

                do
                {
                    if (clickedCard == null)
                        await UniTask.Yield();
                    else
                    {
                        if (!targetList.Contains(clickedCard) && clickedCard != playState.replacedCard)
                            targetList.Add(clickedCard);
                        clickedCard = null;
                    }

                } while (!hasCanceledPlayCard && targetList.Count != playReq);

                uiManager.SetPrompt(false);
                await board.SetOnClickForPlayersUnits(player, CardFactory.Instance.CardPreviewClickHandler);
            }

            async UniTask getTargetsFromCardsInHand(HandManager targetHand, List<CardModel> targetList, int playReq)
            {
                uiManager.SetPrompt(true, $"Select {playReq} " +
                    $"card{(playReq > 1? "s": "")} in " + 
                    $"{(handManager == targetHand? "allied" : "enemy")}" + " hand");
                await targetHand.SetOnClickForCardsInHand(onCardClicked, new List<CardModel>() { playState.card });
                handManager.Hand.ForEach((cardInHand) =>
                {
                    if (cardInHand == playState.card)
                        cardInHand.Playable = false;
                    else
                        cardInHand.Playable = true;
                });

                do
                {
                    if (clickedCard == null)
                        await UniTask.Yield();
                    else
                    {
                        if (!targetList.Contains(clickedCard) && clickedCard != playState.replacedCard)
                            targetList.Add(clickedCard);
                        clickedCard = null;
                    }
                } while (!hasCanceledPlayCard && targetList.Count != playReq);

                uiManager.SetPrompt(false);
                await targetHand.SetOnClickForCardsInHand(CardFactory.Instance.CardPreviewClickHandler, new List<CardModel>() { playState.card });
                handManager.Hand.ForEach((cardInHand) =>
                {
                    if (cardInHand == playState.card)
                        cardInHand.Playable = false;
                    else
                        cardInHand.Playable = true;
                });
            }

            if (playState.card.Type == CardType.Unit && playState.card.SelectedArea.GetIsFull())
            {
                uiManager.SetPrompt(true, $"Select a unit in that row to replace.");
                await board.SetOnClickForUnitRowsUnits(playState.card.SelectedArea, onCardClicked);

                do
                {
                    await UniTask.Yield();
                } while (!hasCanceledPlayCard && clickedCard == null);

                playState.replacedCard = clickedCard;
                clickedCard = null;

                uiManager.SetPrompt(false);
                await board.SetOnClickForUnitRowsUnits(playState.card.SelectedArea, CardFactory.Instance.CardPreviewClickHandler);
            }

            if (playReqs.AllyUnitTargets != 0)
            {
                await getTargetsFromBoard(this, playState.allyUnitTargets, playReqs.AllyUnitTargets);
            }
            if (playReqs.EnemyUnitTargets != 0)
            {
                await getTargetsFromBoard(enemyPlayer, playState.enemyUnitTargets, playReqs.EnemyUnitTargets);
            }
            if (playReqs.AllyCardTargets != 0)
            {
                await getTargetsFromCardsInHand(handManager, playState.allyCardTargets, playReqs.AllyCardTargets);
            }
            if (playReqs.EnemyCardTargets != 0)
            {
                await getTargetsFromCardsInHand(enemyPlayer.handManager, playState.enemyCardTargets, playReqs.EnemyCardTargets);
            }

            if (hasCanceledPlayCard)
            {
                hasCanceledPlayCard = false;
                return false;
            }
        }
        else 
        {
            if (playReqs.AllyUnitTargets > playState.allyUnitTargets.Count)  
                return false;
            if (playReqs.EnemyUnitTargets > playState.enemyUnitTargets.Count)
                return false;
            if (playReqs.AllyCardTargets > playState.allyCardTargets.Count)
                return false;
            if (playReqs.EnemyCardTargets > playState.enemyCardTargets.Count)
                return false;
        }
        

        // The card is committed to being played, so remove it from the hand.
        if (playState.card.Type == CardType.Unit)
        {
            handManager.RemoveCardFromHand(playState.card);
            RefreshPlayableCards();
        }

        await OnBeforeCardPlayed.InvokeAsync(playState);

        if (playState.replacedCard != null)
            await playState.replacedCard.Remove();

        await playState.card.Play(playState);

        await OnAfterCardPlayed.InvokeAsync(playState);


        handManager.SetCardPlayState(null);

        if (playState.card.Type == CardType.Unit) return true;

        if (playState.card.HasCondition("Combust"))
            await DestroyCard(playState.card);
        else
            await DiscardCard(playState.card);

        return true;
    }

    private void CancelPlay()
    {
        Debug.Log("Cancel play called");
        hasCanceledPlayCard = true;
        uiManager.SetRightMiddleButton("End Turn", PassTurn); 
    }
        

    /// <summary>
    /// Method to shuffle the discard pile into the deck.
    /// </summary>
    private void ShuffleDiscardIntoDeck()
    {
        for (int i = 0; i < Discard.Count; i++)
        {
            Discard[i].gameObject.transform.SetParent(deckGameObject.transform, true);
            Deck.Add(Discard[i]);
            RectTransform cardRect = Discard[i].GetComponent<RectTransform>();
            cardRect.anchoredPosition = Vector2.zero;

        }

        Discard.Clear();
        Deck.Shuffle();
    }

    /// <summary>
    /// Method to invoke OnRoundStart event attached to player.
    /// </summary>
    public async UniTask RoundStart(RoundStartState roundStartState)
    {
        await OnRoundStart.InvokeAsync(roundStartState);
    }

    public async UniTask RoundEnd()
    {
        await OnRoundEnd.InvokeAsync();
    }

    /// <summary>
    /// Method to invoke OnUnitSummoned event attached to player.
    /// </summary>
    /// <param name="unit"></param>
    public async UniTask BeforeUnitSummoned(CardModel unit)
    {
        await OnBeforeUnitSummoned.InvokeAsync(unit);
    }

    public async UniTask AfterUnitSummoned(CardModel unit)
    {
        await OnAfterUnitSummoned.InvokeAsync(unit);
    }

    public async UniTask AfterUnitSurvivedDamage(DamageData damageData)
    {
        await OnAfterUnitSurvivedDamage.InvokeAsync(damageData);
    }

    public async UniTask AfterUnitConditionApplied(Condition condition)
    {
        await OnAfterUnitConditionApplied.InvokeAsync(condition);
    }

    public async UniTask UnitDestroyed(CardModel unit)
    {
        await OnUnitDestroyed.InvokeAsync(unit);
    }

}