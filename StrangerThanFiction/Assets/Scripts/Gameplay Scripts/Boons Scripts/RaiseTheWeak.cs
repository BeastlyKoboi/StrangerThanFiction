using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseTheWeak : Boon
{
    public Player player; 

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        this.player.OnRoundEnd.AddListener(AtRoundEndGrantPowerAndPlotArmor);
        return UniTask.CompletedTask;
    }

    private async UniTask AtRoundEndGrantPowerAndPlotArmor()
    {
        CardModel cardModel = player.board.GetWeakestUnit(player);

        if (cardModel != null)
        {
            await cardModel.GrantPower(2);
            await cardModel.GrantPlotArmor(2);
        }
    }
}
