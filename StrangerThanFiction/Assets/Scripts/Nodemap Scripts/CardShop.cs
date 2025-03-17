using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    [SerializeField] private CardDictionary cardDictionary;

    [SerializeField] private int baseCardPrice; 

    [SerializeField] private int baseCommonItemPrice;
    [SerializeField] private int baseUncommonItemPrice;
    [SerializeField] private int baseRareItemPrice;
    [SerializeField] private int baseEpicItemPrice;

    private void Awake()
    {
        Random.InitState(10);
        Random.State state = Random.state;
        string stateSerialized = JsonUtility.ToJson(state);
        Debug.Log(stateSerialized);
        float random = Random.Range(0, 1);

        Debug.Log(random);
        Debug.Log(Random.state);


    }

    public string GetNextPurchaseableCard()
    {

        return "";
    }
}
