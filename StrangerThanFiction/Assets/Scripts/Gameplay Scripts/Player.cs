using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public event Func<Task> OnGameStart;
    public event Func<Task> OnRoundStart;
    public event Func<Task> OnRoundEnd;
    public event Func<Task> OnGameOver;

    public event Func<CardModel, Task> OnCardDrawn;
    public event Func<CardModel, Task> OnUnitSummoned;
    public event Func<CardModel, Task> OnUnitDestroyed;
    public event Func<CardPlayState, Task> OnCardPlayed;
    public event Action OnMyTurnStart;

    [HeaderAttribute("Game and Enemy Info")]
    public GameManager gameManager;
    public UIManager uiManager;
    public BoardManager board;
    public Player enemyPlayer;

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
    public bool hasCardsHidden;
    public int totalDepth = 0;
    public bool hasEndedTurn = false;
    public bool hasEndedRound = false;
    public bool hasCanceledPlayCard = false;

    [HeaderAttribute("The Cards")]
    public HandManager handManager;
    public CardPile Deck { get; private set; }
    public GameObject deckGameObject;

    public CardPile Discard { get; private set; }
    public GameObject discardGameObject;

    [HeaderAttribute("Card Prefabs")]
    public GameObject cardPrefab;
    public GameObject unitPrefab;

    /// <summary>
    /// Method to initialize the player
    /// </summary>
    void Start()
    {
        OnRoundStart += handManager.RoundStart;
        OnRoundEnd += handManager.RoundEnd;
    }

    public void PopulateDeck(DeckInventory deckInventory, bool isHidden)
    {
        hasCardsHidden = isHidden;

        Deck = new CardPile();

        Deck.OnChange += () =>
        {
            uiManager.UpdateDeck(this);
        };

        foreach (DeckEntry entry in deckInventory.deckEntries)
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
        CardModel card = CardFactory.Instance.CreateCard(cardName, hasCardsHidden, deckGameObject.transform, this, board);
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
    public async Task PlayerTurn()
    {
        hasEndedTurn = false;
        bool playedSuccessfully = false;

        uiManager.SetRightMiddleButton("End Turn", PassTurn);

        RefreshPlayableCards();

        OnMyTurnStart?.Invoke();

        do
        {
            while (handManager.PlayState == null && !hasEndedTurn)
            {
                await Task.Yield();
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

        uiManager.SetRightMiddleButton("", () => { });

        handManager.LockCards();
    }

    /// <summary>
    /// Method to draw a card from the player's deck.
    /// </summary>
    public async Task DrawCard()
    {
        if (Deck.Count == 0) ShuffleDiscardIntoDeck();
        if (Deck.Count == 0) return;

        CardModel drawnCard = Deck[Deck.Count - 1];
        Deck.RemoveAt(Deck.Count - 1);

        if (OnCardDrawn != null)
        {
            foreach (Func<CardModel, Task> handler in OnCardDrawn.GetInvocationList()
                .Cast<Func<CardModel, Task>>().ToList())
            {
                await handler(drawnCard);
            }
        }

        handManager.AddCardToHandFromDeck(drawnCard);
        RefreshPlayableCards();
    }

    /// <summary>
    /// Method to discard a card from the player's hand.
    /// </summary>
    /// <param name="card"></param>
    public async Task DiscardCard(CardModel card)
    {
        handManager.RemoveCardFromHand(card);
        RefreshPlayableCards();

        Discard.Add(card);
        card.gameObject.transform.SetParent(discardGameObject.transform, true);

        await card.Discard(this);
    }

    /// <summary>
    /// Method to destroy a card.
    /// </summary>
    /// <param name="card"></param>
    public async Task DestroyCard(CardModel card)
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

    /// <summary>
    /// Method to play a card.
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private async Task<bool> PlayCard(CardPlayState playState)
    {
        // Check if the player has the requirements to play the card.
        PlayRequirements playReqs = playState.card.PlayRequirements;

        if (gameManager.player1 == this)
        {
            uiManager.SetRightMiddleButton("Cancel", CancelPlay);

            CardModel clickedCard = null;
            void onCardClicked(CardModel cardModel)
            {
                clickedCard = cardModel;
            }

            async Task getTargetsFromBoard(Player player, List<CardModel> targetList, int playReq)
            {
                uiManager.SetPrompt(true, $"Select {playReq} " +
                    $"{(gameManager.player1 == player? "allied" : "enemy")} " +
                    $"unit{(playReq > 1? "s": "")}");
                await board.SetOnClickForPlayersUnits(player, onCardClicked);

                do
                {
                    if (clickedCard == null)
                        await Task.Yield();
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

            if (playState.card.Type == CardType.Unit && playState.card.SelectedArea.GetIsFull())
            {
                uiManager.SetPrompt(true, $"Select a unit in that row to replace.");
                await board.SetOnClickForUnitRowsUnits(playState.card.SelectedArea, onCardClicked);

                do
                {
                    await Task.Yield();
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
                // cardPlayState.allyCardTargets = new List<CardModel>();
            }
            if (playReqs.EnemyCardTargets != 0)
            {
                // cardPlayState.enemyCardTargets = new List<CardModel>();
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

        if (OnCardPlayed != null)
        {
            foreach (Func<CardPlayState, Task> handler in OnCardPlayed.GetInvocationList()
                .Cast<Func<CardPlayState, Task>>().ToList())
            {
                await handler(playState);
            }
        }

        if (playState.replacedCard != null)
            await playState.replacedCard.Destroy();

        await playState.card.Play(playState);

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
            Deck.Add(Discard[i]);
            Discard[i].gameObject.transform.SetParent(deckGameObject.transform, true);
            Discard[i].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        Discard.Clear();
        Deck.Shuffle();
    }

    /// <summary>
    /// Method to invoke OnRoundStart event attached to player.
    /// </summary>
    public async Task RoundStart()
    {
        if (OnRoundStart != null)
        {
            foreach (Func<Task> handler in OnRoundStart.GetInvocationList()
                .Cast<Func<Task>>().ToList())
            {
                await handler();
            }
        }
    }

    public async Task RoundEnd()
    {
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
    /// Method to invoke OnUnitSummoned event attached to player.
    /// </summary>
    /// <param name="unit"></param>
    public async Task UnitSummoned(CardModel unit)
    {
        if (OnUnitSummoned != null)
        {
            foreach (Func<CardModel, Task> handler in OnUnitSummoned.GetInvocationList()
                .Cast<Func<CardModel, Task>>().ToList())
            {
                await handler(unit);
            }
        }
    }

    public async Task UnitDestroyed(CardModel unit)
    {
        if (OnUnitDestroyed != null)
        {
            foreach (Func<CardModel, Task> handler in OnUnitDestroyed.GetInvocationList()
                .Cast<Func<CardModel, Task>>().ToList())
            {
                await handler(unit);
            }
        }
    }

}