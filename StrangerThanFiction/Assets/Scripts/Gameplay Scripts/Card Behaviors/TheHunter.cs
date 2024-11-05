using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheHunter : CardModel
{
    public override uint Id => 17;

    protected override async Task PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.allyUnitTargets[0].Strike(cardPlayState.enemyUnitTargets[0]);

        await cardPlayState.allyUnitTargets[0].Destroy();
    }
}
