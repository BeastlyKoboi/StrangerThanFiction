using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeMenu : MonoBehaviour, IDataPersistence
{
    public NodemapManager nodemapManager;
    public SceneLoader sceneLoader;
    private GameData _gameData;
    public GameObject mainMenu;

    public CanvasGroup mainMenuCanvasGroup;
    public TextMeshProUGUI title;
    public TextMeshProUGUI flavor;
    public TextMeshProUGUI description;
    public Button mainBtn;
    public TextMeshProUGUI mainBtnText;

    
    

    public void OpenMenu(MapNode mapNode)
    {
        mainMenuCanvasGroup.alpha = 1;
        mainMenuCanvasGroup.interactable = true;
        mainMenuCanvasGroup.blocksRaycasts = true;

        // 
        NodeData nodeData = mapNode.GetNodeData();

        title.text = nodeData.Title;
        description.text = nodeData.Description;

        mainBtn.onClick.RemoveAllListeners();


        if (nodeData is BattleNodeData)
        {
            BattleNodeData battleNodeData = (BattleNodeData)nodeData;
            mainBtnText.text = "Battle";
            flavor.text = "";

            mainBtn.onClick.AddListener(() =>
            {
                _gameData.player2Deck.deckEntries.Clear();

                foreach (DeckEntry entry in battleNodeData.DeckInventory.deckEntries)
                {
                    _gameData.player2Deck.deckEntries.Add(new DeckEntry(entry.cardName, entry.numCopies));
                }

                // scene loader

                sceneLoader.LoadScene("Gameplay");

            });
        } 
        else if (nodeData is SpecialNodeData)
        {
            SpecialNodeData specialNodeData = (SpecialNodeData)nodeData;
            mainBtnText.text = "Confirm";

            CardDataMono cardDataMono = GameObject.Find("CardData").GetComponent<CardDataMono>();

            CardInfo newCard;
            int numLoops = 0;
            do
            {
                newCard = cardDataMono.cardDictionary.GetCardDataByIndex(Random.Range(0, cardDataMono.cardDictionary.cardEntries.Count - 1));

                Debug.Log(_gameData.player1Deck.deckEntries.Count);

                foreach (DeckEntry entry in _gameData.player1Deck.deckEntries)
                {
                    if (entry.cardName == newCard.name)
                    {
                        newCard = null;
                        break;
                    }
                }
                numLoops++;

            } while (newCard == null || newCard.Type == CardType.Spell || numLoops > 20);

            if (numLoops < 20)
            {
                flavor.text = "You encountered " + newCard.Title;
                flavor.text += "\n" + newCard.EncounterText;
                _gameData.player1Deck.deckEntries.Add(new DeckEntry(newCard.name, 1));

            }
            
            mainBtn.onClick.AddListener(() =>
            {
                CloseMenu();
                nodemapManager.SetSelectableNodes();
            });
        }

    }

    public void CloseMenu()
    {
        mainMenuCanvasGroup.alpha = 0;
        mainMenuCanvasGroup.interactable = false;
        mainMenuCanvasGroup.blocksRaycasts = false;
    }

    public void LoadData(GameData data)
    {
        _gameData = data;
    }

    public void SaveData(GameData data)
    {
        
    }
}
