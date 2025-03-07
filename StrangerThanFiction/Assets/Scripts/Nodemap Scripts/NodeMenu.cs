using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeMenu : MonoBehaviour, IDataPersistence
{
    public NodemapManager nodemapManager;
    public SceneLoader sceneLoader;
    private GameData _gameData;
    public GameObject mainMenu;


    [Header("Shop Menu")]
    [SerializeField] private CanvasGroup shopMenuCanvasGroup;
    [SerializeField] private ShopUI shopUI;

    public void LoadData(GameData data)
    {
        _gameData = data;
    }

    public void SaveData(GameData data)
    {
        
    }

    public void OpenShopMenu(DeusShop deusShop)
    {
        shopMenuCanvasGroup.alpha = 1;
        shopMenuCanvasGroup.interactable = true;
        shopMenuCanvasGroup.blocksRaycasts = true;

        shopUI.PopulateShop(deusShop);
    }


    
}
