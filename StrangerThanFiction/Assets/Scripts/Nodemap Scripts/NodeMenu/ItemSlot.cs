using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, SelectableSlot
{
    public Item item;
    [SerializeField] private GameObject conditionsBox;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemDescText;

    public void SetItemDescText(Item item)
    {
        this.item = item;
        icon.sprite = item.Icon;
        itemDescText.text = item.ToString();
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
