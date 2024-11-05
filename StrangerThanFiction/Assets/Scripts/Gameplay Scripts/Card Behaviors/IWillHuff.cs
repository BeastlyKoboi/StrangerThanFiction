using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class IWillHuff : CardModel
{
    public override uint Id => 19;

    public override async void Start()
    {
        base.Start();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async Task PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyUnitTargets[0].TakeDamage(1);

        Owner.CreateCardInDeck("AndIWillPuff");
        Owner.CreateCardInDeck("AndIWillPuff");
    }
}
