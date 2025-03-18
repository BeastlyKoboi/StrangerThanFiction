using Cysharp.Threading.Tasks;
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
    [SerializeField] private Button rerollBtn;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject cardZone;

    private CardShop cardShop;

    private DeusShop deusShop;

    [SerializeField] private RunInfo runInfo;

    private void Awake()
    {
        cardShop = GetComponent<CardShop>();

        rerollBtn.onClick.AddListener(() =>
        {
            RerollShop().Forget();
        });

        buyBtn.onClick.AddListener(() =>
        {
            // Buy item
        });

        closeBtn.onClick.AddListener(() =>
        {
            CloseShop();
        });
    }

    public void Initialize(DeusShop deusShop)
    {
        this.deusShop = deusShop;

        if (!deusShop.isShopOpen)
        {
            PopulateShop();
            deusShop.isShopOpen = true;
        }
    }

    public void PopulateShop()
    {
        foreach (Transform child in cardZone.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++)
        {
            string cardName = cardShop.GetNextPurchaseableCard();
            CardFactory.Instance.CreateNonPlayableCard(cardName, false, cardZone.transform);
        }
    }

    public async UniTask RerollShop()
    {
        if (runInfo.GetRerollTokens() <= 0) return; 

        await runInfo.SetRerollTokens(runInfo.GetRerollTokens() - 1);
        deusShop.IncrementRerollTokensUsed();
        PopulateShop();
    }


    public void CloseShop()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

}
