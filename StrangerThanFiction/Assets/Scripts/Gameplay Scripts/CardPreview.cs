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
    public Transform conditionsBox;
    public Transform flavorBox;
    public TextMeshProUGUI cardTextCost;
    public TextMeshProUGUI cardTextPower;
    public TextMeshProUGUI cardTextPlotArmor;
    public TextMeshProUGUI flavorText;
    private Sprite spellCardFrame;
    private Sprite unitCardFrame;

    [SerializeField] private GameObject conditionBoxPrefab;

    private void Start()
    {
        cardView = transform.Find("Sample Card");
        blurPanel = transform.Find("Blur");
        conditionsBox = transform.Find("Conditions");
        cardTextCost = cardView.Find("Cost").GetComponent<TextMeshProUGUI>();
        cardTextPower = cardView.Find("Power").GetComponent<TextMeshProUGUI>();
        cardTextPlotArmor = cardView.Find("PlotArmor").GetComponent<TextMeshProUGUI>();

        flavorBox = transform.Find("Flavor");
        flavorText = flavorBox.Find("FlavorText").GetComponent<TextMeshProUGUI>();
    }

    public void Unfocus()
    {
        cardView.gameObject.SetActive(false);
        blurPanel.gameObject.SetActive(false);
        conditionsBox.gameObject.SetActive(false);
        flavorBox.gameObject.SetActive(false);

        // Clear all children from conditionsBox
        foreach (Transform child in conditionsBox)
        {
            Destroy(child.gameObject);
        }

        flavorText.text = "";
    }

    public void OnClick(CardModel cardScript)
    {
        //Debug.Log($"{cardScript.name} was clicked.");
        OverwriteCardPreview(cardScript);
        cardView.gameObject.SetActive(true);
        blurPanel.gameObject.SetActive(true);
        conditionsBox.gameObject.SetActive(true);
        flavorBox.gameObject.SetActive(true);
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
        Sprite sprite = card.Portrait;
        Transform portrait = cardView.Find("Portrait");

        if (sprite != null)
            portrait.GetComponent<Image>().sprite = sprite;

        cardTextCost.text = card.CurrentCost.ToString();

        Transform spellBackground = cardView.Find("SpellBackground");
        Transform unitBackground = cardView.Find("UnitBackground");

        if (card.Type == CardType.Spell)
        {
            spellBackground.gameObject.SetActive(true);
            unitBackground.gameObject.SetActive(false);

            cardView.Find("Power").gameObject.SetActive(false);
            cardView.Find("PlotArmor").gameObject.SetActive(false);
        }
        else
        {
            unitBackground.gameObject.SetActive(true);
            spellBackground.gameObject.SetActive(false);

            cardTextPower.text = card.CurrentPower.ToString();
            cardTextPlotArmor.text = card.CurrentPlotArmor.ToString();

            cardView.Find("Power").gameObject.SetActive(true);
            cardView.Find("PlotArmor").gameObject.SetActive(true);
        }

        cardView.Find("Name").GetComponent<TextMeshProUGUI>().text = card.Title;
        cardView.Find("Description").GetComponent<TextMeshProUGUI>().text = card.Description;
        cardView.Find("Cardback").gameObject.SetActive(card.IsHidden);

        if (card.IsHidden)
        {
            return;
        }

        Condition[] conditions = card.GetConditions();

        for (int i = 0; i < conditions.Length; i++)
        {
            GameObject conditionBox = Instantiate(conditionBoxPrefab, conditionsBox);
            conditionBox.transform.Translate(0, i * -175,0);
            conditionBox.transform.Find("Description").GetComponent<TextMeshProUGUI>()
                .text = conditions[i].ToString();
        }

        flavorText.text = card.FlavorText;
    }
}
