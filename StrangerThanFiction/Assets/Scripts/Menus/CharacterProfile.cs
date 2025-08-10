using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private Faction faction;
    [SerializeField] private List<DeckEntry> characterDeck;
    [SerializeField] public bool hasData;
    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private GameObject continueGameButton;

    public void SetData(RunData runData)
    {
        if (runData == null || !runData.runHasStarted || runData.runHasEnded)
        {
            hasData = false;
            startBtnText.text = "Start";
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

    public List<DeckEntry> GetCharacterDeck()
    {
        return characterDeck;
    }

}
