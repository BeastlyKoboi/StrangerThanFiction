using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningBlow : Item
{
    public OpeningBlow(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {
    }

    public override UniTask OnAdd()
    {
        card.OnSummon.AddListener(StrikeWeakestEnemy);
        return base.OnAdd();
    }

    public override UniTask OnRemove()
    {
        card.OnSummon.RemoveListener(StrikeWeakestEnemy);
        return base.OnRemove();
    }

    private async UniTask StrikeWeakestEnemy()
    {
        CardModel weakestEnemy = card.Owner.board.GetWeakestUnit(card.Owner.enemyPlayer);

        if (weakestEnemy)
        {
            await card.Strike(weakestEnemy);
        }
    }

}
