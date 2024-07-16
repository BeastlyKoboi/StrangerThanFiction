using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardPreview : MonoBehaviour
{
    public Transform cardView;
    public Transform blurPanel;
    public TextMeshProUGUI cardTextCost;
    public TextMeshProUGUI cardTextPower;
    public TextMeshProUGUI cardTextPlotArmor;
    public Image cardBackground;
    private Sprite spellCardFrame;
    private Sprite unitCardFrame;

    private void Start()
    {
        cardView = transform.Find("Sample Card");
        blurPanel = transform.Find("Blur");
        cardTextCost = cardView.Find("Cost").GetComponent<TextMeshProUGUI>();
        cardTextPower = cardView.Find("Power").GetComponent<TextMeshProUGUI>();
        cardTextPlotArmor = cardView.Find("PlotArmor").GetComponent<TextMeshProUGUI>();
        cardBackground = cardView.Find("Background").GetComponent<Image>();

        spellCardFrame = CardModel.LoadSprite("SpellCardFrontFrame.png");
        unitCardFrame = CardModel.LoadSprite("UnitCardFrontFrame.png");
    }

    public void Unfocus()
    {
        cardView.gameObject.SetActive(false);
        blurPanel.gameObject.SetActive(false);
    }

    public void OnClick(CardModel cardScript)
    {
        Debug.Log($"{cardScript.name} was clicked.");
        OverwriteCardPreview(cardScript);
        cardView.gameObject.SetActive(true);
        blurPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// At instantiation will be used to overwrite placeholder card gameobject
    /// with correct sprites and initial values.
    /// </summary>
    public void OverwriteCardPreview(CardModel card)
    {
        if (cardView == null)
            return;

        // Load portrait picture
        Sprite sprite = CardModel.LoadSprite(card.PortraitPath);
        Transform portrait = cardView.Find("Portrait");

        if (sprite != null)
            portrait.GetComponent<Image>().sprite = sprite;

        cardTextCost.text = card.CurrentCost.ToString();

        // Placeholder has Unit Card frame automatically, so replace it if needed
        if (card.Type == CardType.Spell)
        {
            cardBackground.sprite = spellCardFrame;

            cardView.Find("Power").gameObject.SetActive(false);
            cardView.Find("PlotArmor").gameObject.SetActive(false);
        }
        else
        {
            cardBackground.sprite = unitCardFrame;

            cardTextPower.text = card.CurrentPower.ToString();
            cardTextPlotArmor.text = card.CurrentPlotArmor.ToString();

            cardView.Find("Power").gameObject.SetActive(true);
            cardView.Find("PlotArmor").gameObject.SetActive(true);
        }

        cardView.Find("Name").GetComponent<TextMeshProUGUI>().text = card.Title;
        cardView.Find("Description").GetComponent<TextMeshProUGUI>().text = card.Description;

        if (card.IsHidden)
            cardView.Find("Cardback").gameObject.SetActive(true);
    }
}