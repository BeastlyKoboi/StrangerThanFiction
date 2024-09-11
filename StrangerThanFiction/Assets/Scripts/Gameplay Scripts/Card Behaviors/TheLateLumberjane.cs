using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheLateLumberjane : CardModel
{
    public override uint Id => 9;

    protected override Task SummonEffect()
    {
        Owner.OnUnitDestroyed += OnDestroyEffect;
        Owner.enemyPlayer.OnUnitDestroyed += OnDestroyEffect;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Owner.OnUnitDestroyed -= OnDestroyEffect;
        Owner.enemyPlayer.OnUnitDestroyed -= OnDestroyEffect;
        return Task.CompletedTask;
    }

    private async Task OnDestroyEffect(CardModel unit)
    {
        await GrantPower(2);
    }
}
