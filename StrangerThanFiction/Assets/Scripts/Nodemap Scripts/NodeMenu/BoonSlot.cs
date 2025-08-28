using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoonSlot : MonoBehaviour, SelectableSlot
{
    public Boon boon;
    [SerializeField] private GameObject boonTilePrefab;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;

    public void SetBoonText(Boon boon)
    {
        this.boon = boon;
        this.titleText.text = boon.Name;
        this.descText.text = boon.Description;
    }

    public void ToggleSelected(bool isSelected)
    {
        if (isSelected)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
