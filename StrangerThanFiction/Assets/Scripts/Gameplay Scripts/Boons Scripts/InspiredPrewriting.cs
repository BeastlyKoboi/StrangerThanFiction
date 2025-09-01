using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspiredPrewriting : Boon
{
    private Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        this.player.OnRoundStart.AddListener(GrantHighestCostCardInDeckCostReduction);
        return UniTask.CompletedTask;
    }

    private async UniTask GrantHighestCostCardInDeckCostReduction(RoundStartState roundStartState)
    {
        CardModel highestCard = null;
        for (int i = 0; i < player.Deck.Count; i++)
        {
            if (highestCard == null || player.Deck[i].CurrentCost > highestCard.CurrentCost)
            {
                highestCard = player.Deck[i];
            }
        }
        await highestCard.GrantCostModification(-1);
    }
}
