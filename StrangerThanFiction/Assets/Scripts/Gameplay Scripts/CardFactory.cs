using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardFactory
{
    private static CardFactory instance;
    public static CardFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CardFactory();
            }
            return instance;
        }
    }

    // Make the constructor private so it can't be instantiated outside of this class
    private CardFactory() { }

    private GameObject cardStorage;
    private CardDataMono cardData;
    private ItemDataMono itemData;

    // The script that handles card previews
    public CardPreview cardPreview;

    // Example properties for prefabs
    public GameObject cardPrefab;
    public GameObject unitPrefab;

    private Queue<GameObject> lockedSpellCardPool;
    private Queue<GameObject> lockedUnitCardPool;

    private Queue<GameObject> spellCardPool;
    private Queue<GameObject> unitCardPool;

    private Dictionary<Player, DeckInventory> registeredDecks = new Dictionary<Player, DeckInventory>();

    private RunInfo runInfo;

    // Example method to set prefabs if needed
    public void Initialize()
    {
        this.cardStorage = GameObject.Find("Card Storage");
        this.cardPreview = GameObject.Find("CardPreview").GetComponent<CardPreview>();
        this.lockedSpellCardPool = new Queue<GameObject>();
        this.lockedUnitCardPool = new Queue<GameObject>();
        this.spellCardPool = new Queue<GameObject>();
        this.unitCardPool = new Queue<GameObject>();

        if (cardData == null)
            cardData = GameObject.Find("CardData").GetComponent<CardDataMono>();
        this.cardPrefab = cardData.cardPrefab;
        this.unitPrefab = cardData.unitPrefab;

        if (itemData == null)
            itemData = GameObject.Find("ItemData").GetComponent<ItemDataMono>();

        if (runInfo == null)
            runInfo = GameObject.Find("RunData").GetComponent<RunDataMono>().runInfo;
    }

    public void RegisterDeck(Player player, DeckInventory deck)
    {
        if (!registeredDecks.ContainsKey(player))
            registeredDecks.Add(player, deck);
    }

    public CardModel CreateCard(Type type, bool isHidden, Transform parent, Player owner, BoardManager board, string creator = "")
    {
        string cardName = type.Name;
        return CreateCard(cardName, isHidden, parent, owner, board, creator);
    }

    public CardModel CreateCard(string cardName, bool isHidden, Transform parent, Player owner, BoardManager board, string creator = "")
    {
        GameObject cardObj = new GameObject(cardName, typeof(RectTransform));
        cardObj.transform.SetParent(parent, false);
        RectTransform cardRect = cardObj.GetComponent<RectTransform>();
        RectTransform prefabRect = cardPrefab.GetComponent<RectTransform>();
        Vector2 size = cardRect.sizeDelta;
        size.x = prefabRect.rect.width;
        size.y = prefabRect.rect.height;
        cardRect.sizeDelta = size;

        if (cardData == null)
            cardData = GameObject.Find("CardData").GetComponent<CardDataMono>();

        CardInfo cardInfo = cardData.cardDictionary.GetByKey(cardName);

        GameObject queuedCard = GetCardFromPool(cardInfo.Type);

        if (queuedCard)
        {
            LinkHandlerForTMPTextHover hoverHandler = queuedCard.GetComponentInChildren<LinkHandlerForTMPTextHover>();
            if (hoverHandler != null)
                hoverHandler.enabled = true;

            while (queuedCard.transform.childCount != 0)
            {
                queuedCard.transform.GetChild(0).SetParent(cardObj.transform, false);
            }

            GameObject.Destroy(queuedCard);
        }
        else
        {
            GameObject instantiatedCardPrefab = GameObject.Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            instantiatedCardPrefab.transform.SetParent(cardObj.transform, false);

            if (cardInfo.Type == CardType.Unit)
            {
                GameObject instantiatedUnitPrefab = GameObject.Instantiate(unitPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                instantiatedUnitPrefab.transform.SetParent(cardObj.transform, false);
            }
        }

        cardObj.AddComponent<CardView>();
        CardView cardView = cardObj.GetComponent<CardView>();
        cardView.Instantiate(cardInfo);

        Type cardScriptType = Type.GetType(cardName);
        if (cardScriptType != null)
        {
            cardObj.AddComponent(cardScriptType);
        }

        CardModel cardScript = cardObj.GetComponent<CardModel>();
        cardScript.IsHidden = isHidden;
        cardScript.Owner = owner;
        cardScript.Board = board;

        cardView.OverwriteCardPrefab(cardScript);

        if (cardScript.Type == CardType.Unit)
            cardView.OverwriteUnitPrefab(cardScript);

        cardObj.AddComponent<Clickable>();
        cardObj.GetComponent<Clickable>().OnClickWithoutDrag += CardPreviewClickHandler;


        DeckInventory deckInventory;

        if (registeredDecks.ContainsKey(owner))
        {
            deckInventory = registeredDecks[owner];
        }
        else
        {
            deckInventory = runInfo.GetDeckInventory();
        }

        foreach (DeckEntry entry in deckInventory.GetDeckEntries())
        {
            if (entry.cardName == cardName)
            {
                foreach (string itemString in entry.items)
                {
                    ItemInfo itemInfo = itemData.itemDictionary.GetByKey(itemString);
                    Type itemScript = Type.GetType(itemString);
                    Item item = (Item)Activator.CreateInstance(itemScript, itemInfo, cardScript);
                    cardScript.AddItem(item);
                }

                break;
            }
        }

        return cardScript;
    }

    public CardModel CreateNonPlayableCard(string cardName, bool isHidden, Transform parent, string creator = "")
    {
        GameObject cardObj = new GameObject(cardName, typeof(RectTransform));
        cardObj.transform.SetParent(parent, false);

        if (cardData == null)
            cardData = GameObject.Find("CardData").GetComponent<CardDataMono>();

        CardInfo cardInfo = cardData.cardDictionary.GetByKey(cardName);

        GameObject queuedCard = GetCardFromPool(cardInfo.Type);

        if (queuedCard)
        {
            LinkHandlerForTMPTextHover hoverHandler = queuedCard.GetComponentInChildren<LinkHandlerForTMPTextHover>();
            if (hoverHandler != null)
                hoverHandler.enabled = true;
            
            while (queuedCard.transform.childCount != 0)
            {
                queuedCard.transform.GetChild(0).SetParent(cardObj.transform, false);
            }

            GameObject.Destroy(queuedCard);
        }
        else
        {
            GameObject instantiatedCardPrefab = GameObject.Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            instantiatedCardPrefab.transform.SetParent(cardObj.transform, false);

            if (cardInfo.Type == CardType.Unit)
            {
                GameObject instantiatedUnitPrefab = GameObject.Instantiate(unitPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                instantiatedUnitPrefab.transform.SetParent(cardObj.transform, false);
            }
        }

        cardObj.AddComponent<CardView>();
        CardView cardView = cardObj.GetComponent<CardView>();
        cardView.Instantiate(cardInfo);

        Type cardScriptType = Type.GetType(cardName);
        if (cardScriptType != null)
        {
            cardObj.AddComponent(cardScriptType);
        }

        CardModel cardScript = cardObj.GetComponent<CardModel>();
        cardScript.IsHidden = isHidden;

        cardView.OverwriteCardPrefab(cardScript);

        if (cardScript.Type == CardType.Unit)
            cardView.OverwriteUnitPrefab(cardScript);

        cardObj.AddComponent<Clickable>();
        cardObj.GetComponent<Clickable>().OnRightClick += CardPreviewClickHandler;
        cardObj.GetComponent<Clickable>().OnLongClick += CardPreviewClickHandler;

        DeckInventory deckInventory = runInfo.GetDeckInventory();

        foreach (DeckEntry entry in deckInventory.GetDeckEntries())
        {
            if (entry.cardName == cardName)
            {
                foreach (string itemString in entry.items)
                {
                    ItemInfo itemInfo = itemData.itemDictionary.GetByKey(itemString);
                    Type itemScript = Type.GetType(itemString);
                    Item item = (Item)Activator.CreateInstance(itemScript, itemInfo, cardScript);
                    cardScript.AddItem(item);
                }

                break;
            }
        }

        return cardScript;
    }

    public void CardPreviewClickHandler(CardModel cardModel) { if (cardPreview) cardPreview.OnClick(cardModel); }

    // Additional methods for card comparison or other functionalities can be added here.


    private GameObject GetCardFromPool(CardType cardType)
    {
        if (cardType == CardType.Spell && spellCardPool.Count > 0)
            return spellCardPool.Dequeue();
        else if (cardType == CardType.Unit && unitCardPool.Count > 0)
            return unitCardPool.Dequeue();
        return null;
    }

    public UniTask RecycleCard(CardModel card)
    {
        GameObject cardObj = card.gameObject;
        CardType cardType = card.Type;

        cardObj.transform.SetParent(cardStorage.transform, false);

        // Disable hover and interaction scripts
        LinkHandlerForTMPTextHover hoverHandler = cardObj.GetComponentInChildren<LinkHandlerForTMPTextHover>();
        if (hoverHandler != null)
            hoverHandler.enabled = false;

        // Optionally, disable the whole card GameObject if you don't need it active
        // cardObj.SetActive(false);

        if (cardType == CardType.Unit)
            lockedUnitCardPool.Enqueue(cardObj);
        else
            lockedSpellCardPool.Enqueue(cardObj);

        return UniTask.CompletedTask;
    }
     
    public UniTask QueueLockedCards()
    {
        while (lockedUnitCardPool.Count > 0)
            unitCardPool.Enqueue(lockedUnitCardPool.Dequeue());
        while (lockedSpellCardPool.Count > 0)
            spellCardPool.Enqueue(lockedSpellCardPool.Dequeue());

        return UniTask.CompletedTask;
    }
}
