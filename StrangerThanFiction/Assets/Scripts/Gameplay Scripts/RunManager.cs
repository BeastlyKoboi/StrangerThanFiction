using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour, IDataPersistence
{
    private NodemapManager nodemapManager;

    private DeckInventory deckInventory;

    [SerializeField] private GameObject deckPageContent;
    [SerializeField] private GameObject powerPageContent;
    [SerializeField] private GameObject questsPageContent;

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
        deckInventory = data.player1Deck;

        foreach (DeckEntry entry in deckInventory.deckEntries)
        {
            for (int i = 0; i < entry.numCopies; i++)
            {
                CardFactory.Instance.CreateNonPlayableCard(entry.cardName, false, deckPageContent.transform);
            }
        }
    }

    public void SaveData(GameData data)
    {

    }
}
