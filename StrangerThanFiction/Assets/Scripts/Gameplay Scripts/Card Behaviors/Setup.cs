using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Setup : CardModel
{
    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        foreach (CardModel unit in Board.GetUnits(Owner))
        {
            await unit.GrantPlotArmor(3);
        }

        Owner.CreateCardInDeck(typeof(Payoff).ToString());
    }
}
