using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public event Action OnGameStart;
    public event Func<Task> OnRoundStart;
    public event Func<Task> OnRoundEnd;
    public event Action OnGameOver;

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

    /// <summary>
    /// Method to initialize the player's deck.
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="isHidden"></param>
    public void PopulateDeck(string[] cards, bool isHidden)
    {
        hasCardsHidden = isHidden;

        Deck = new CardPile();
        //Deck.OnChange += UpdateDeck;
        Deck.OnChange += () =>
        {
            uiManager.UpdateDeck(this);
        };

        foreach (string card in cards)
        {
            Deck.Add(CardFactory.Instance.CreateCard(card, isHidden, deckGameObject.transform, this, board));
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

    public CardModel CreateCardinDiscard(string cardName)
    {
        CardModel card = CardFactory.Instance.CreateCard(cardName, hasCardsHidden, deckGameObject.transform, this, board);
        Discard.Add(card);
        Discard.Shuffle();
        return card;
    }

    /// <summary>
    /// Method to handle the player's turn. 
    /// </summary>
    /// <returns></returns>
    public async Task PlayerTurn()
    {
        hasEndedTurn = false;

        handManager.RefreshPlayableCards();

        OnMyTurnStart?.Invoke();

        handManager.RefreshPlayableCards();

        while (handManager.playedCard == null && !hasEndedTurn)
        {
            await Task.Yield();
        }

        if (handManager.playedCard)
        {
            await PlayCard(handManager.playedCard);

            uiManager.UpdateTotalPower();
        }

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
                .Cast<Func<CardModel, Task>>())
            {
                await handler(drawnCard);
            }
        }

        handManager.AddCardToHandFromDeck(drawnCard);
    }

    /// <summary>
    /// Method to discard a card from the player's hand.
    /// </summary>
    /// <param name="card"></param>
    public async Task DiscardCard(CardModel card)
    {
        handManager.RemoveCardFromHand(card);

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
        handManager.RefreshPlayableCards();
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
    private async Task PlayCard(CardModel card)
    {
        CardPlayState cardPlayState = new CardPlayState(
            card: card,
            player: this,
            enemy: enemyPlayer,
            allyUnitTargets: null,
            enemyUnitTargets: null,
            allyCardTargets: null,
            enemyCardTargets: null
        );

        if (card.Type == CardType.Unit)
            handManager.RemoveCardFromHand(card);

        if (OnCardPlayed != null)
        {
            foreach (Func<CardPlayState, Task> handler in OnCardPlayed.GetInvocationList()
                .Cast<Func<CardPlayState, Task>>())
            {
                await handler(cardPlayState);
            }
        }

        await card.Play(cardPlayState);

        handManager.playedCard = null;

        if (card.Type == CardType.Unit) return;

        if (card.HasCondition(Combust.GetName()))
            await DestroyCard(card);
        else
            await DiscardCard(card);

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
                .Cast<Func<Task>>())
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
                .Cast<Func<Task>>())
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
                .Cast<Func<CardModel, Task>>())
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
                .Cast<Func<CardModel, Task>>())
            {
                await handler(unit);
            }
        }
    }

}