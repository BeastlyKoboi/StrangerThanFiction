using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public CardInfo CardInfo;

    public Transform cardTransform;
    private TextMeshProUGUI cardTextCost;
    private TextMeshProUGUI cardTextPower;
    private TextMeshProUGUI cardTextPlotArmor;
    private TextMeshProUGUI cardTitle;
    private TextMeshProUGUI cardDescription;
    private Image cardPortrait;
    private Image cardback;
    private GameObject cardGlow;

    public Transform unitTransform;
    private TextMeshProUGUI unitTextPower;
    private TextMeshProUGUI unitTextPlotArmor;
    private TextMeshProUGUI unitTitle;
    private Transform unitConditionIconsBox;
    private GameObject unitConditionIconPrefab;
    private Image unitContestedIcon;
    private Image unitUncontestedIcon; 


    public void Instantiate(CardInfo cardInfo)
    {
        CardInfo = cardInfo;

        cardTransform = transform.Find("CardPrefab(Clone)");
        cardTextCost = cardTransform.Find("Cost").GetComponent<TextMeshProUGUI>();
        cardTextPower = cardTransform.Find("Power").GetComponent<TextMeshProUGUI>();
        cardTextPlotArmor = cardTransform.Find("PlotArmor").GetComponent<TextMeshProUGUI>();
        cardTitle = cardTransform.Find("Name").GetComponent<TextMeshProUGUI>();
        cardDescription = cardTransform.Find("Description").GetComponent<TextMeshProUGUI>();
        cardPortrait = cardTransform.Find("Portrait").GetComponent<Image>();
        cardback = cardTransform.Find("Cardback").GetComponent<Image>();
        cardGlow = cardTransform.Find("Glow").gameObject;
        cardTransform.gameObject.SetActive(true);

        unitTransform = transform.Find("UnitPrefab(Clone)");
        if (unitTransform != null)
        {
            unitTextPower = unitTransform.Find("Power").GetComponent<TextMeshProUGUI>();
            unitTextPlotArmor = unitTransform.Find("PlotArmor").GetComponent<TextMeshProUGUI>();
            unitTitle = unitTransform.Find("Name").GetComponent<TextMeshProUGUI>();
            unitTransform.gameObject.SetActive(false);
            unitConditionIconsBox = unitTransform.Find("Conditions");
            unitConditionIconPrefab = unitTransform.Find("ConditionIcon").gameObject;

            unitContestedIcon = unitTransform.Find("contested").GetComponent<Image>();
            unitUncontestedIcon = unitTransform.Find("uncontested").GetComponent<Image>();
        }

        cardTextCost.text = cardInfo.BaseCost.ToString();
        cardTextPower.text = cardInfo.BasePower.ToString();
        cardTextPlotArmor.text = cardInfo.BasePlotArmor.ToString();
        cardTitle.text = cardInfo.Title;
        cardDescription.text = cardInfo.Description;
        cardPortrait.sprite = cardInfo.Portrait;
        cardback.gameObject.SetActive(false);
    }

    public void UpdateCardView(CardModel cardModel)
    {
        cardTextCost.text = cardModel.CurrentCost.ToString();
        cardTextPower.text = cardModel.CurrentPower.ToString();
        cardTextPlotArmor.text = cardModel.CurrentPlotArmor.ToString();
        cardTitle.text = cardModel.Title;
        cardDescription.text = cardModel.Description;
        cardPortrait.sprite = cardModel.Portrait;
    }

    public void OverwriteCardPrefab(CardModel cardModel)
    {
        cardTransform = transform.Find("CardPrefab(Clone)");
        cardTransform.gameObject.SetActive(true);

        if (cardTransform == null)
            return;

        cardPortrait.sprite = cardModel.Portrait;
        cardTextCost.text = cardModel.CurrentCost.ToString();


        Transform spellBackground = cardTransform.Find("SpellBackground");
        Transform unitBackground = cardTransform.Find("UnitBackground");

        if (cardModel.Type == CardType.Spell)
        {
            spellBackground.gameObject.SetActive(true);
            unitBackground.gameObject.SetActive(false);

            cardTextPower.gameObject.SetActive(false);
            cardTextPlotArmor.gameObject.SetActive(false);
        }
        else
        {
            unitBackground.gameObject.SetActive(true);
            spellBackground.gameObject.SetActive(false);

            cardTextPower.text = cardModel.CurrentPower.ToString();
            cardTextPlotArmor.text = cardModel.CurrentPlotArmor.ToString();
        }

        cardTitle.text = cardModel.Title;
        cardDescription.text = cardModel.Description;
        cardback.gameObject.SetActive(cardModel.IsHidden);
    }

    /// <summary>
    /// At instantiation will be used to overwrite placeholder unit gameobject
    /// with correct sprites and initial values. 
    /// </summary>
    public void OverwriteUnitPrefab(CardModel cardModel)
    {
        unitTransform = transform.Find("UnitPrefab(Clone)");

        if (unitTransform == null)
            return;

        // Load portrait picture
        Sprite sprite = cardModel.Portrait;
        Transform portrait = unitTransform.Find("Portrait");

        if (sprite != null)
            portrait.GetComponent<Image>().sprite = sprite;

        unitTextPower.text = cardModel.CurrentPower.ToString();
        unitTextPlotArmor.text = cardModel.CurrentPlotArmor.ToString();
        unitTitle.text = cardModel.Title;
        unitTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the card stat text on the card view.
    /// </summary>
    public void UpdateCardStatText(CardModel cardModel)
    {
        if (!cardTransform) return;
        cardTextCost.text = cardModel.CurrentCost.ToString();

        // Spells need to be updated, but will not have these saved. 
        if (cardTextPower)
        {
            cardTextPower.text = cardModel.CurrentPower.ToString();
            if (cardModel.CurrentPower > cardModel.BasePower)
                cardTextPower.color = Color.green;
            else
                cardTextPower.color = Color.white;
        }
        if (cardTextPlotArmor)
        {
            cardTextPlotArmor.text = cardModel.CurrentPlotArmor.ToString();
            if (cardModel.CurrentPlotArmor > cardModel.BasePlotArmor)
                cardTextPlotArmor.color = Color.green;
            else
                cardTextPlotArmor.color = Color.white;
        }
    }

    /// <summary>
    /// Updates the unit stat text on the unit view.
    /// </summary>
    public void UpdateUnitStatText(CardModel cardModel)
    {
        if (!unitTransform) return;
        unitTextPower.text = cardModel.CurrentPower.ToString();
        unitTextPlotArmor.text = cardModel.CurrentPlotArmor.ToString();

        if (cardModel.CurrentPower < cardModel.MaxPower)
            unitTextPower.color = Color.red;
        else if (cardModel.CurrentPower > cardModel.BasePower)
            unitTextPower.color = Color.green;
        else
            unitTextPower.color = Color.white;

        if (cardModel.CurrentPlotArmor > cardModel.BasePlotArmor)
            unitTextPlotArmor.color = Color.green;
        else if (cardModel.CurrentPlotArmor < cardModel.BasePlotArmor)
            unitTextPlotArmor.color = Color.yellow;
        else
            unitTextPlotArmor.color = Color.white;
    }

    public void UpdateConditionsBox(Condition[] conditions)
    {
        if (!unitTransform) return;

        // Clear all children from conditionsBox
        foreach (Transform child in unitConditionIconsBox)
        {
            Destroy(child.gameObject);
        }

        foreach (Condition condition in conditions)
        {
            Image icon = Instantiate(unitConditionIconPrefab, unitConditionIconsBox).GetComponent<Image>();
            icon.sprite = condition.Icon;
        }
    }

    public void ToggleGlow(bool enabled)
    {
        cardGlow.SetActive(enabled);
    }

    public void ToggleViewType(bool isUnit)
    {
        cardTransform.gameObject.SetActive(!isUnit);
        unitTransform.gameObject.SetActive(isUnit);
    }

    public void SetCardbackVisibility(bool isVisible)
    {
        if (cardback != null)
        {
            cardback.gameObject.SetActive(isVisible);
        }
    }

    public void ToggleIsUnitContested(bool isContested, bool isVisible = true)
    {
        if (!unitContestedIcon || !unitUncontestedIcon) return;
        if (isContested)
        {
            unitContestedIcon.gameObject.SetActive(true);
            unitUncontestedIcon.gameObject.SetActive(false);
        }
        else
        {
            unitContestedIcon.gameObject.SetActive(false);
            unitUncontestedIcon.gameObject.SetActive(true);
        }

        if (!isVisible) 
        {
            unitContestedIcon.gameObject.SetActive(false);
            unitUncontestedIcon.gameObject.SetActive(false);
        }
    }
}
