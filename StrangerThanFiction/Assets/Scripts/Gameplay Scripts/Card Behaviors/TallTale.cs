using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class TallTale : CardModel
{
    public override uint Id => 7;

    // Start is called before the first frame update
    public override async void Start()
    {
        base.Start();

        await ApplyCondition(new Combust(this, 0));
    }

    protected override async Task PlayEffect()
    {
        if (Owner.Deck.Count > 0)
        {
            CardModel topUnit = Owner.Deck[^1];

            await topUnit.GrantCostModification(-1);
        }
    }
}
