using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MagicalWoodcarving : CardModel
{
    protected override UniTask SummonEffect()
    {
        OnRoundEnd += RoundEndEffect;
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundEnd -= RoundEndEffect;
        return UniTask.CompletedTask;
    }

    protected async UniTask RoundEndEffect()
    {
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (randomEnemy)
        {
            StartCoroutine(GetComponent<UnitAnim>().Strike(1.0f));
            await Strike(randomEnemy);
        }

        await Destroy();
    }
}
