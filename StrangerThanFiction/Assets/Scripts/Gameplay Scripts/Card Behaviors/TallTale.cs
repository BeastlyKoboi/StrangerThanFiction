using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class TallTale : CardModel
{
    // Start is called before the first frame update
    protected override async void Awake()
    {
        base.Awake();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        //if (Owner.Deck.Count > 0)
        //{
        //    CardModel topUnit = Owner.Deck[^1];

        //    await topUnit.GrantCostModification(-1);
        //}

        if (cardPlayState.allyCardTargets.Count > 0)
        {
            await cardPlayState.allyCardTargets[0].GrantCostModification(-1);
            Owner.MoveCardFromHandToDeck(cardPlayState.allyCardTargets[0], moveToTop: true, shuffleAfter: false);
        }

    }
}
