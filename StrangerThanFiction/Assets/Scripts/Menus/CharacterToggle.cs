using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterToggle : MonoBehaviour, ISelectHandler, IDeselectHandler, IDataPersistence
{
    public Image panel; 
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public Color panelColor;
    public string characterName;
    [TextArea]
    public string characterDescription;

    [SerializeField] private Faction faction;
    [SerializeField] private List<DeckEntry> characterDeck;
    private RunData runData;

    [SerializeField] CharacterProfile characterProfile;

    public void OnSelect(BaseEventData eventData)
    {
        panel.color = panelColor;
        title.text = characterName;
        description.text = characterDescription;

        characterProfile.SetFaction(faction);
        characterProfile.SetCharacterDeck(characterDeck);
        characterProfile.SetData(runData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        
    }

    public void LoadData(GameData data)
    {
        if (faction == Faction.LittleRed)
            runData = data.littleRedRunData;
        else if (faction == Faction.Pinocchio)
            runData = data.pinocchioRunData;
        else if (faction == Faction.HumptyDumpty)
            runData = data.humptyDumptyRunData;
       
    }

    public void SaveData(GameData data)
    {

    }
}
