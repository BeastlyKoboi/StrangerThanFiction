using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";
    [SerializeField] public bool hasData;
    [SerializeField] private TextMeshProUGUI startBtnText;
    [SerializeField] private GameObject newGameButton;

    public void SetData(GameData gameData)
    {
        if (gameData == null || gameData.runHasEnded)
        {
            hasData = false;
            startBtnText.text = "Start";
        }
        else
        {
            hasData = true;
            startBtnText.text = "Continue";
            newGameButton.SetActive(true);
        }
    }

    public string GetProfileId()
    {
        return profileId;
    }

}
