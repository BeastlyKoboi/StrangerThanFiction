using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour, IDataPersistence
{
    private NodemapManager nodemapManager;

    [SerializeField] private GameObject deckPageContent;
    [SerializeField] private GameObject powerPageContent;
    [SerializeField] private GameObject questsPageContent;

    [Header("Player Data")]
    [SerializeField] private RunInfo runInfo;

    private void Awake()
    {
        
        nodemapManager = GetComponent<NodemapManager>();
        CardFactory.Instance.Initialize();

    }

    private void OnEnable()
    {
        runInfo.OnAfterDeckInventoryChange.AddListener(RefreshDeck);
    }

    private void OnDisable()
    {
        runInfo.OnAfterDeckInventoryChange.RemoveListener(RefreshDeck);
    }

    private UniTask RefreshDeck(EventState eventState = null)
    {
        foreach (Transform child in deckPageContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (DeckEntry entry in runInfo.GetDeckInventory().deckEntries)
        {
            for (int i = 0; i < entry.numCopies; i++)
            {
                CardFactory.Instance.CreateNonPlayableCard(entry.cardName, false, deckPageContent.transform);
            }
        }
        return UniTask.CompletedTask;
    }

    public void LoadData(GameData data)
    {
        runInfo.SetCurrency(data.currency).Forget();
        runInfo.SetRerollTokens(data.rerollTokens).Forget();
        runInfo.SetDeckInventory(data.player1Deck).Forget();

        RefreshDeck();
    }

    public void SaveData(GameData data)
    {
        data.currency = runInfo.GetCurrency();
        data.rerollTokens = runInfo.GetRerollTokens();
        data.player1Deck = runInfo.GetDeckInventory();
    }
}
