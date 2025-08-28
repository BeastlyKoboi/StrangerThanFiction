using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestOfCharacterUI : EncounterUI
{
    [SerializeField] private GameObject compatibilityIcon;

    public void SetCompatibilityLabel(bool isCompatible)
    {
        compatibilityIcon.SetActive(true);

        Transform redPanel = compatibilityIcon.transform.Find("RedPanel");
        Transform greenPanel = compatibilityIcon.transform.Find("GreenPanel");
        TextMeshProUGUI label = compatibilityIcon.GetComponentInChildren<TextMeshProUGUI>();

        if (isCompatible)
        {
            greenPanel.gameObject.SetActive(true);
            redPanel.gameObject.SetActive(false);
            label.text = "Compatible!";
        }
        else
        {
            greenPanel.gameObject.SetActive(false);
            redPanel.gameObject.SetActive(true);
            label.text = "Incompatible";
        }
    }


}
