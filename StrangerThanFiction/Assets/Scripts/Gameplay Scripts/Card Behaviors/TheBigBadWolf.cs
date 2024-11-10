using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheBigBadWolf : CardModel
{
    protected override UniTask SummonEffect()
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
        CardModel crow = CardFactory.Instance.CreateCard("Crow", true, transform, Owner, Board, Title);
        await crow.Summon();
    }
}