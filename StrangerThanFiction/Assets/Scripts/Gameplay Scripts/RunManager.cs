using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RunManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject deckPageContent;
    [SerializeField] private GameObject powerPageContent;
    [SerializeField] private GameObject questsPageContent;

    [Header("Player Data")]
    [SerializeField] private RunInfo runInfo;

    [Header("Boon Data")]
    [SerializeField] private GameObject boonPrefab;
    public BoonCollection BoonCollection { get; set; } = new BoonCollection();


    private void Awake()
    {
        CardFactory.Instance.Initialize();
    }

    private void OnEnable()
    {
        runInfo.OnAfterDeckInventoryChange.AddListener(RefreshDeck);
        runInfo.OnAfterBoonListChange.AddListener(RefreshBoons);
    }

    private void OnDisable()
    {
        runInfo.OnAfterDeckInventoryChange.RemoveListener(RefreshDeck);
        runInfo.OnAfterBoonListChange.RemoveListener(RefreshBoons);
    }

    private UniTask RefreshDeck(EventState eventState = null)
    {
        foreach (Transform child in deckPageContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (DeckEntry entry in runInfo.GetDeckInventory().GetDeckEntries())
        {
            for (int i = 0; i < entry.numCopies; i++)
            {
                CardFactory.Instance.CreateNonPlayableCard(entry.cardName, false, deckPageContent.transform);
            }
        }
        return UniTask.CompletedTask;
    }

    private UniTask RefreshBoons(EventState eventState = null)
    {
        BoonCollection = new BoonCollection(); // Reset the boon collection

        foreach (Transform child in powerPageContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (string boonName in runInfo.GetBoonList())
        {
            Type boonType = Type.GetType(boonName);
            Boon boon = (Boon)Activator.CreateInstance(boonType);
            BoonCollection.AddBoon(boon);

            GameObject boonObject = Instantiate(boonPrefab, powerPageContent.transform);
            boonObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = boon.Name;
            boonObject.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = boon.Description;
        }
        return UniTask.CompletedTask;
    }


    public void LoadData(GameData data)
    {
        runInfo.SetCurrency(data.GetRunData().currency).Forget();
        runInfo.SetRerollTokens(data.GetRunData().rerollTokens).Forget();
        runInfo.SetDeckInventory(data.GetRunData().player1Deck).Forget();
        runInfo.SetBoonList(data.GetRunData().boonList).Forget();

        RefreshDeck();
        RefreshBoons();
    }

    public void SaveData(GameData data)
    {
        data.GetRunData().currency = runInfo.GetCurrency();
        data.GetRunData().rerollTokens = runInfo.GetRerollTokens();
        data.GetRunData().player1Deck = runInfo.GetDeckInventory();
        data.GetRunData().boonList = runInfo.GetBoonList();
    }
}
