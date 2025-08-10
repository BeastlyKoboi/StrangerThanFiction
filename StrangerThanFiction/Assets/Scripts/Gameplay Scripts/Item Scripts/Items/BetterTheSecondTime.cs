using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BetterTheSecondTime : Item
{
    public BetterTheSecondTime(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {
    }

    public override UniTask OnAdd()
    {
        card.OnSummon.AddListener(IfCopyOnBoardGrantPower);
        return UniTask.CompletedTask;
    }

    public override UniTask OnRemove()
    {
        card.OnSummon.RemoveListener(IfCopyOnBoardGrantPower);
        return UniTask.CompletedTask;
    }

    private async UniTask IfCopyOnBoardGrantPower()
    {
        CardModel[] units = card.Board.GetUnits(card.Owner);

        foreach (CardModel unit in units)
        {
            if (unit.GetType().Name == card.GetType().Name && unit != card)
            {
                await card.GrantPower(3);
                break;
            }
        }

    }
}
