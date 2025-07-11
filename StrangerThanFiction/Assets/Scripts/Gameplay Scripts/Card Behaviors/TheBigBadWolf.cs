using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheBigBadWolf : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState)
    {
        Owner.OnUnitDestroyed.AddListener(SummonCrow);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitDestroyed.RemoveListener(SummonCrow);
        return UniTask.CompletedTask;
    }

    private async UniTask SummonCrow(CardModel card)
    {
        if (card == this)
            return;
        CardModel crow = CardFactory.Instance.CreateCard("Crow", true, transform, Owner, Board, Title);
        await crow.Deploy();
        await crow.Summon();
    }
}