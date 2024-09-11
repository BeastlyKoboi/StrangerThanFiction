using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WittyWhittler : CardModel
{
    public override uint Id => 11;

    protected override Task SummonEffect()
    {
        Owner.OnUnitSummoned += OnAllySummonedGrantPoison;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnUnitSummoned -= OnAllySummonedGrantPoison;
        return Task.CompletedTask;
    }

    public async Task OnAllySummonedGrantPoison(CardModel ally)
    {
        CardModel enemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (enemy)
            await enemy.ApplyCondition(new Poisoned(enemy, 1));
    }

}
