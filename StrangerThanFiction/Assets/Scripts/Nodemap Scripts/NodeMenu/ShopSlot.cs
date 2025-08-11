using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    private Transform cardParent;
    [SerializeField] private GameObject conditionsBox;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI itemDescText;
    private TextMeshProUGUI priceText;

    private DeckEntry deckEntry;
    private int price;

    private void Awake()
    {
        cardParent = transform.Find("CardParent");
        priceText = transform.Find("Price").GetComponent<TextMeshProUGUI>();
    }

    public void SetDeckEntry(DeckEntry deckEntry)
    {
        this.deckEntry = deckEntry;
    }

    public DeckEntry GetDeckEntry()
    {
        return deckEntry;
    }

    public Transform GetCardParent()
    {
        return cardParent;
    }

    public void SetItemDescText(Item item)
    {
        icon.sprite = item.Icon;
        itemDescText.text = item.ToString();
    }

    public void ToggleItemDescText(bool isActive)
    {
        conditionsBox.SetActive(isActive);
    }

    public void SetPriceText(int price) 
    {
        this.price = price;
        priceText.text = "$" + price.ToString();
    }

    public int GetPrice()
    {
        return price;
    }

    public void ToggleSelected(bool isSelected)
    {
        if (isSelected)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
