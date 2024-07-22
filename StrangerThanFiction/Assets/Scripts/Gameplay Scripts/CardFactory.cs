using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    // The script that handles card previews
    public CardPreview cardPreview;

    // Example properties for prefabs
    public GameObject cardPrefab;
    public GameObject unitPrefab;

    private Queue<GameObject> spellCardPool = new Queue<GameObject>();
    private Queue<GameObject> unitCardPool = new Queue<GameObject>();


    // Example method to set prefabs if needed
    public void Initialize(GameObject cardPrefab, GameObject unitPrefab)
    {
        this.cardStorage = GameObject.Find("Card Storage");
        this.cardPrefab = cardPrefab;
        this.unitPrefab = unitPrefab;
        this.cardPreview = GameObject.Find("CardPreview").GetComponent<CardPreview>();
    }

    public CardModel CreateCard(string cardName, bool isHidden, Transform parent, Player owner, BoardManager board, string creator = "")
    {
        GameObject cardObj = new GameObject(cardName, typeof(RectTransform));
        cardObj.transform.SetParent(parent, false);

        Type cardScriptType = Type.GetType(cardName);
        if (cardScriptType != null)
        {
            cardObj.AddComponent(cardScriptType);
        }

        CardModel cardScript = cardObj.GetComponent<CardModel>();
        cardScript.IsHidden = isHidden;
        cardScript.Owner = owner;
        cardScript.Board = board;

        
        GameObject queuedCard = GetCardFromPool(cardScript.Type);

        if (queuedCard)
        {
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

            if (cardScript.Type == CardType.Unit)
            {
                GameObject instantiatedUnitPrefab = GameObject.Instantiate(unitPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                instantiatedUnitPrefab.transform.SetParent(cardObj.transform, false);
            }
        }

        cardScript.OverwriteCardPrefab();

        if (cardScript.Type == CardType.Unit)
            cardScript.OverwriteUnitPrefab();

        cardObj.AddComponent<Clickable>();
        cardObj.GetComponent<Clickable>().OnClickWithoutDrag += () =>
        {
            cardPreview.OnClick(cardScript);
        };

        return cardScript;

    }

    // Additional methods for card comparison or other functionalities can be added here.
    
    
    private GameObject GetCardFromPool(CardType cardType)
    {
        if (cardType == CardType.Spell && spellCardPool.Count > 0)
            return spellCardPool.Dequeue();
        else if (cardType == CardType.Unit && unitCardPool.Count > 0)
            return unitCardPool.Dequeue();
        return null;
    }

    public Task RecycleCard(CardModel card)
    {
        GameObject cardObj = card.gameObject;
        CardType cardType = card.Type;

        cardObj.transform.SetParent(cardStorage.transform, false);
        cardObj.SetActive(false);

        if (cardType == CardType.Unit)
            unitCardPool.Enqueue(cardObj);
        else
            spellCardPool.Enqueue(cardObj);

        // Check how this affects the card's functionality
        //Component[] comps = cardObj.GetComponents(typeof(Component));
        //foreach (Component comp in comps)
        //{
        //    if (comp.GetType() != typeof(RectTransform))
        //        GameObject.Destroy(comp);
        //}

        return Task.CompletedTask;
    }
}
