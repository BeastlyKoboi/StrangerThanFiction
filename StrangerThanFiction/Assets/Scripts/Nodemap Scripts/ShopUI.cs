using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI shopDescriptionText;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject cardZone;

    private void Awake()
    {
        buyBtn.onClick.AddListener(() =>
        {
            // Buy item
        });

        closeBtn.onClick.AddListener(() =>
        {
            CloseShop();
        });
    }


    public void PopulateShop(DeusShop deusShop)
    {


    }


    public void CloseShop()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

}
