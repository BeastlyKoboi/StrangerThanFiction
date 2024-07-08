using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Example properties for prefabs
    public GameObject cardPrefab;
    public GameObject unitPrefab;
    public GameObject discardPrefab;

    // Example method to set prefabs if needed
    public void Initialize(GameObject cardPrefab, GameObject unitPrefab, GameObject discardPrefab)
    {
        this.cardPrefab = cardPrefab;
        this.unitPrefab = unitPrefab;
        this.discardPrefab = discardPrefab;
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

        GameObject instantiatedCardPrefab = GameObject.Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        instantiatedCardPrefab.transform.SetParent(cardObj.transform, false);

        CardModel cardScript = cardObj.GetComponent<CardModel>();
        cardScript.IsHidden = isHidden;
        cardScript.Owner = owner;
        cardScript.Board = board;

        cardScript.OverwriteCardPrefab();

        if (cardScript.Type == CardType.Unit)
        {
            GameObject instantiatedUnitPrefab = GameObject.Instantiate(unitPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            instantiatedUnitPrefab.transform.SetParent(cardObj.transform, false);
            cardScript.OverwriteUnitPrefab();
        }


        return cardScript;
    }

    // Additional methods for card comparison or other functionalities can be added here.

}
