using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AndIWillPuff : CardModel
{
    public override uint Id => 20;

    public override async void Start()
    {
        base.Start();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async Task PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(2);
        await cardPlayState.enemyUnitTargets[1].TakeDamage(2);

        Owner.CreateCardInDeck("AndIWillBlowYouAway");
        Owner.CreateCardInDeck("AndIWillBlowYouAway");
    }
}
