using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Rascals : CardModel
{
    protected async override void Awake()
    {
        base.Awake();

        await ApplyCondition(new Stackable(this, 0, new List<string>() { "Donkey" }));
    }

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnRoundStart.AddListener(CreateTrickOrTreatInHand);
        return base.DeployEffect(deployState);
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnRoundStart.RemoveListener(CreateTrickOrTreatInHand);
        return base.RemoveEffect(card);
    }

    private UniTask CreateTrickOrTreatInHand()
    {
        Owner.CreateCardInHand("TallTale");
        return UniTask.CompletedTask;
    }
}
