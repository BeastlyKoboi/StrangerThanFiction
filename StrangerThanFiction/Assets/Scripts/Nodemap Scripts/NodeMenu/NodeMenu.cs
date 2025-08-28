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
    [SerializeField] private EncounterUI deusShopUI;
    [SerializeField] private EncounterUI theCuttingRoomUI;
    [SerializeField] private EncounterUI testOfCharacterUI;


    public void LoadData(GameData data)
    {

    }

    public void SaveData(GameData data)
    {
        
    }

    public EncounterUI GetNodeUI(SpecialNode node)
    {
        if (node is DeusShop) return deusShopUI;
        if (node is TheCuttingRoom) return theCuttingRoomUI;
        if (node is TestOfCharacter) return testOfCharacterUI;
        return null;
    }

    public RunInfo GetRunInfo() => runInfo;

    public CardShop GetCardShop() => cardShop;



}
