using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheBigBadWolf : CardModel
{
    public override uint Id => 15;

    protected override Task SummonEffect()
    {
        Owner.enemyPlayer.OnUnitDestroyed += SummonCrow;
        Owner.OnUnitDestroyed += SummonCrow;
        return Task.CompletedTask;
    }

    protected override Task RemoveEffect(CardModel card)
    {
        Owner.enemyPlayer.OnUnitDestroyed -= SummonCrow;
        Owner.OnUnitDestroyed -= SummonCrow;
        return Task.CompletedTask;
    }

    private async Task SummonCrow(CardModel card)
    {
        CardModel crow = CardFactory.Instance.CreateCard("Crow", true, transform, Owner, Board, Title);
        await crow.Summon();
    }
}