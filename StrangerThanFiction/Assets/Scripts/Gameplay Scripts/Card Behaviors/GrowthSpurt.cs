using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class GrowthSpurt : CardModel
{
    public override uint Id => 3;

    protected override async Task PlayEffect(CardPlayState cardPlayState)
    {
        CardModel weakestUnit = cardPlayState.allyUnitTargets[0];// Board.GetWeakestUnit(Owner);

        if (weakestUnit)
        {
           await weakestUnit.GrantPower(4);
        }
    }

}
