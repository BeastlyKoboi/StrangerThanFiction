using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private Faction faction;
    [SerializeField] private List<DeckEntry> characterDeck;
    [SerializeField] public bool hasData;
    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private GameObject continueGameButton;
    [SerializeField] private TMP_InputField seedInputField;


    public void SetData(RunData runData)
    {
        if (runData == null || !runData.runHasStarted || runData.runHasEnded)
        {
            hasData = false;
            startBtnText.text = "Start";
            continueGameButton.SetActive(false);
        }
        else
        {
            hasData = true;
            startBtnText.text = "New Game";
            continueGameButton.SetActive(true);
        }
    }

    public Faction GetFaction()
    {
        return faction;
    }

    public void SetFaction(Faction faction)
    {
        this.faction = faction;
    }

    public List<DeckEntry> GetCharacterDeck()
    {
        return characterDeck;
    }

    public void SetCharacterDeck(List<DeckEntry> characterDeck)
    {
        this.characterDeck = characterDeck;
    }

    public string GetSeed()
    {
        if (seedInputField != null && !string.IsNullOrEmpty(seedInputField.text))
            return seedInputField.text;
        return null;
    }

}
