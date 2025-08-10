using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TheCornKennel : CardModel
{
    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnRoundEnd.AddListener(IfAllyDamagedThisRoundSummonPupcorn);
        return base.DeployEffect(deployState);
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundEnd.RemoveListener(IfAllyDamagedThisRoundSummonPupcorn);
        return base.RemoveEffect(card);
    }

    private async UniTask IfAllyDamagedThisRoundSummonPupcorn()
    {
        if (Owner.NumUnitsSurvivedDamageThisRound > 0)
        {
            CardModel donkey = CardFactory.Instance.CreateCard(typeof(Pupcorn).ToString(), true, transform, Owner, Board, Title);
            await donkey.Deploy();
            await donkey.Summon();
        }
    }
}
