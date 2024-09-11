using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MagicalWoodcarving : CardModel
{
    public override uint Id => 4;

    protected override Task SummonEffect()
    {
        OnRoundEnd += RoundEndEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        OnRoundEnd -= RoundEndEffect;
        return Task.CompletedTask;
    }

    protected async Task RoundEndEffect()
    {
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (randomEnemy)
        {
            StartCoroutine(GetComponent<UnitAnim>().Strike(1.0f));
            await randomEnemy.TakeDamage(CurrentPower);
        }

        await Destroy();
    }
}
