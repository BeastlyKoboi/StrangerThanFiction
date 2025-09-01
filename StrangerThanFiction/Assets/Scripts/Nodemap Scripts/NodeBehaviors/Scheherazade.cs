using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheherazade : SpecialNode
{
    protected override void PopulateUI()
    {
        PopulateUIState state = new PopulateUIState();

        List<BoonInfo> boons = new List<BoonInfo>();

        boons = nodeMenu.GetBoonShop().GetUniqueRandomBoons(3);

        for (int i = 0; i < boons.Count; i++)
        {
            state.options.Add(new EncounterOption(EncounterOptionType.Boon, boons[i]));
        }

        state.rerollCost = GetNextRerollCost();

        nodeUI.PopulateUI(state);
    }
 
    protected override async UniTask Confirm(ConfirmSelectState confirmSelectState)
    {
        Debug.Log("Confirming Scheherazade selection");
        Debug.Log(confirmSelectState.boonSlot);

        if (confirmSelectState.boonSlot == null) return;
        
        await runInfo.AddBoon(confirmSelectState.boonSlot.boon.GetType().ToString());

        nodeUI.CloseUI();
        mapNode.ToggleSelectable(false);
    }
}
