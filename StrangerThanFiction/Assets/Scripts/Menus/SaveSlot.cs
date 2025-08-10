using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";
    [SerializeField] public bool hasData;
    [SerializeField] private GameObject selectSlotButton;
    [SerializeField] private TextMeshProUGUI selectSlotBtnText;

    public void ToggleSelected(bool isSelected)
    {
        transform.localScale = isSelected ? Vector3.one * 1.2f : Vector3.one;
    }

    public void SetData(GameData gameData)
    {
        if (gameData == null)
        {
            hasData = false;
            selectSlotBtnText.text = "Empty";
        }
        else
        {
            hasData = true;
            selectSlotBtnText.text = profileId;
        }
    }

    public string GetProfileId()
    {
        return profileId;
    }
}
