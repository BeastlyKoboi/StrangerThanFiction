using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NothingLeftUnwritten : Boon
{
    private Player player;

    public override UniTask EnterCombat(Player player)
    {
        this.player = player;
        player.OnRoundEnd.AddListener(IfNoInkLeftSummonTopUnitFromDeck);
        return UniTask.CompletedTask;
    }

    private async UniTask IfNoInkLeftSummonTopUnitFromDeck()
    {
        if (player.CurrentMana <= 0 && player.board.playerRow.GetUnits().Length <= 6)
        {
            CardModel topUnit = null;

            for (int i = player.Deck.Count - 1; i >= 0; i--)
            {
                if (player.Deck[i].Type == CardType.Unit)
                {
                    topUnit = player.Deck[i];
                    player.Deck.RemoveAt(i);
                    break;
                }
            }

            if (topUnit != null)
            {
                await topUnit.Deploy();
                await topUnit.Summon();
            }
        }
    }
}
