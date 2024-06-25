using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// My thinking is that all player input will be filtered in from here, human or AI. 
/// The player's UI will have access to their player and the AI will have access 
/// to their players methods behind the scenes. Upon player activity the game manager 
/// will prompt and manage the actions (somehow). 
/// </summary>

public class Player : MonoBehaviour
{
    public event Action OnGameStart;
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action OnGameOver;

    public event Action<CardModel> OnUnitSummoned;
    public event Action<CardModel> OnCardPlayed;
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

    }

    /// <summary>
    /// Method to initialize the player's deck.
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="isHidden"></param>
    public void PopulateDeck(string[] cards, bool isHidden)
    {
        Deck = new CardPile();
        //Deck.OnChange += UpdateDeck;
        Deck.OnChange += () =>
        {
            uiManager.UpdateDeck(this);
        };

        foreach (string card in cards)
        {
            Deck.Add(CreateCard(card, isHidden, deckGameObject));
        }

        Deck.Shuffle();

        Discard = new CardPile();
        Discard.OnChange += () =>
        {
            uiManager.UpdateDiscard(this);
        };
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
            Debug.Log("Card Played");
            await PlayCard(handManager.playedCard);

            uiManager.UpdateTotalPower();
        }

        handManager.LockCards();
    }

    /// <summary>
    /// Method to draw a card from the player's deck.
    /// </summary>
    public void DrawCard()
    {
        if (Deck.Count == 0) ShuffleDiscardIntoDeck();
        if (Deck.Count == 0) return;

        CardModel drawnCard = Deck[Deck.Count - 1];
        Deck.RemoveAt(Deck.Count - 1);
        handManager.AddCardToHandFromDeck(drawnCard);
    }

    /// <summary>
    /// Method to discard a card from the player's hand.
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(CardModel card)
    {
        handManager.RemoveCardFromHand(card);
        Discard.Add(card);
        card.gameObject.transform.SetParent(discardGameObject.transform, true);
        RectTransform cardRect = card.gameObject.GetComponent<RectTransform>();
        cardRect.anchoredPosition = Vector2.zero;
        cardRect.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Method to destroy a card.
    /// </summary>
    /// <param name="card"></param>
    public async void DestroyCard(CardModel card)
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
        OnCardPlayed?.Invoke(card);
        await card.Play(this);

        if (card.Type == CardType.Unit)
            handManager.RemoveCardFromHand(card);
        else
        {
            if (card.HasCondition(Combust.GetName())) 
                DestroyCard(card);
            else 
                DiscardCard(card);
        }

        handManager.playedCard = null;
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
    public void RoundStart()
    {
        OnRoundStart?.Invoke();
    }

    /// <summary>
    /// Method to invoke OnUnitSummoned event attached to player.
    /// </summary>
    /// <param name="unit"></param>
    public void UnitSummoned(CardModel unit)
    {
        OnUnitSummoned?.Invoke(unit);
    }

    /// <summary>
    /// Method to create a card.
    /// Probably should be moved to a factory class.
    /// </summary>
    /// <param name="cardName"></param>
    /// <param name="isHidden"></param>
    /// <param name="parent"></param>
    /// <param name="creator"></param>
    /// <returns></returns>
    public CardModel CreateCard(string cardName, bool isHidden, GameObject parent, string creator = "")
    {
        GameObject cardObj = new GameObject(cardName, typeof(RectTransform));
        cardObj.transform.SetParent(parent.transform, false);

        Type cardScriptType = Type.GetType(cardName);
        if (cardScriptType != null)
        {
            cardObj.AddComponent(cardScriptType);
        }

        Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity).transform.SetParent(cardObj.transform, false);

        CardModel cardScript = cardObj.GetComponent<CardModel>();
        cardScript.IsHidden = isHidden;
        cardScript.Owner = this;
        cardScript.Board = board;

        cardScript.OverwriteCardPrefab();

        if (cardScript.Type == CardType.Unit)
        {
            Instantiate(unitPrefab, new Vector3(0, 0, 0), Quaternion.identity).transform.SetParent(cardObj.transform, false);
            cardScript.OverwriteUnitPrefab();
        }

        return cardScript;
    }

}