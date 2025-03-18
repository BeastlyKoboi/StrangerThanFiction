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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(GameData data)
    {
        runInfo.SetCurrency(data.currency).Forget();
        runInfo.SetRerollTokens(data.rerollTokens).Forget();
        runInfo.SetDeckInventory(data.player1Deck).Forget();


        foreach (DeckEntry entry in runInfo.GetDeckInventory().deckEntries)
        {
            for (int i = 0; i < entry.numCopies; i++)
            {
                CardFactory.Instance.CreateNonPlayableCard(entry.cardName, false, deckPageContent.transform);
            }
        }


    }

    public void SaveData(GameData data)
    {
        data.currency = runInfo.GetCurrency();
        data.rerollTokens = runInfo.GetRerollTokens();
        data.player1Deck = runInfo.GetDeckInventory();
    }
}
