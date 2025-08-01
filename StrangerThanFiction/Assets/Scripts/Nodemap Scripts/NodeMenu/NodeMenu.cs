using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeMenu : MonoBehaviour, IDataPersistence
{
    public NodemapManager nodemapManager;
    [SerializeField] private RunInfo runInfo;
    [SerializeField] private CardShop cardShop;

    [Header("Shop Menu")]
    [SerializeField] private CanvasGroup shopMenuCanvasGroup;
    [SerializeField] private ShopUI deusShopUI;
    [SerializeField] private ShopUI theCuttingRoomUI;


    public void LoadData(GameData data)
    {

    }

    public void SaveData(GameData data)
    {
        
    }

    public ShopUI GetNodeUI(SpecialNode node)
    {
        if (node is DeusShop) return deusShopUI;
        if (node is TheCuttingRoom) return theCuttingRoomUI;
        return null;
    }

    public RunInfo GetRunInfo() => runInfo;

    public CardShop GetCardShop() => cardShop;



}
