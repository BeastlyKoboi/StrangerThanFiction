using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeusShop : MonoBehaviour, IDataPersistence
{
    private GameData gameData;
    public CanvasGroup shopCanvasGroup;




    public void OpenShop()
    {
        Debug.Log("Shop Opened");

        shopCanvasGroup.alpha = 1;
        shopCanvasGroup.interactable = true;

    }

    public void LoadData(GameData data)
    {
        this.gameData = data;
    }

    public void SaveData(GameData data)
    {

    }

}
