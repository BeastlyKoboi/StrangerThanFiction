using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public sealed class TallTale : CardModel
{
    public override string Title => "Tall Tale";
    public override string Description => "The top card on deck get -1 cost. On play I am destroyed.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Spell;
    public override string PortraitPath => "CardPortraits/Tall_Tale.png";

    public override int BaseCost => 0;


    // Start is called before the first frame update
    public override async void Start()
    {
        base.Start();

        await ApplyCondition(Combust.StaticName, new Combust(this, 0));
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
