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

    public void SetData(GameData gameData)
    {
        if (gameData == null)
        {
            hasData = false;
            startBtnText.text = "Start";
        }
        else
        {
            hasData = true;
            startBtnText.text = "Continue";
        }
    }

    public string GetProfileId()
    {
        return profileId;
    }

}
