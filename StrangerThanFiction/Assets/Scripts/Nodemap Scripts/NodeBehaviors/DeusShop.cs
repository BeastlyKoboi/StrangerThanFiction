using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeusShop : SpecialNode
{
    private GameData gameData;

    public bool isShopOpen = false;

    private void Start()
    {
        mapNode.AddOnClick((MapNode mapNode) => { nodeMenu.OpenShopMenu(this); });
    }


    public override void LoadData(GameData data)
    {
        this.gameData = data;
    }

    public override void SaveData(GameData data)
    {

    }

}
