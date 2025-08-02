using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeekInheritance : Boon
{
    public Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        player.OnBeforeUnitSummoned.AddListener(BuffLowCostUnits);
        return UniTask.CompletedTask;
    }
    
    public override UniTask ExitCombat()
    {
        player.OnBeforeUnitSummoned.RemoveListener(BuffLowCostUnits);
        return UniTask.CompletedTask;
    }

    private async UniTask BuffLowCostUnits(CardModel unit)
    {
        if (unit != null && unit.CurrentCost <= 1)
        {
            await unit.GrantPower(2);
        }
    }
}
