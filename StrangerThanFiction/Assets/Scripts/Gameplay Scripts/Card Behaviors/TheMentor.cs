using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public sealed class TheMentor : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnRoundStart.AddListener(SummonAndDefendTheChosenOne);

        return UniTask.CompletedTask;
    }

    protected override async UniTask SummonEffect()
    {
        await SummonAndDefendTheChosenOne(new RoundStartState());
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart.RemoveListener(SummonAndDefendTheChosenOne);

        return UniTask.CompletedTask;
    }

    private async UniTask SummonAndDefendTheChosenOne(RoundStartState roundStartState)
    {
        CardModel theChosenOne = null;
        theChosenOne = Board.GetUnits(Owner).FirstOrDefault(unit => unit is TheChosenOne);

        if (theChosenOne != null)
        {
            await theChosenOne.GrantPlotArmor(2);
        }
        else
        {
            theChosenOne = CardFactory.Instance.CreateCard(typeof(TheChosenOne).ToString(), true, transform, Owner, Board, Title);
            await theChosenOne.Deploy();
            await theChosenOne.Summon();
        }
    }
}
