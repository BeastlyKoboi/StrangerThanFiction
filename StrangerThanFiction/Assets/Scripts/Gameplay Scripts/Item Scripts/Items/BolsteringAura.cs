using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BolsteringAura : Item
{
    public BolsteringAura(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {
    }

    public override UniTask OnAdd()
    {
        card.OnSummon.AddListener(GrantAllAlliesPower);
        return UniTask.CompletedTask;
    }

    public override UniTask OnRemove()
    {
        card.OnSummon.RemoveListener(GrantAllAlliesPower);
        return UniTask.CompletedTask;
    }

    private async UniTask GrantAllAlliesPower()
    {
        CardModel[] allies = card.Owner.board.GetUnits(card.Owner);
        foreach (CardModel ally in allies)
        {
            await ally.GrantPower(1);
        }
    }
}
