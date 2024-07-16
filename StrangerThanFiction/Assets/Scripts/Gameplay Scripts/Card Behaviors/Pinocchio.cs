using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Represents the functionality of Pinocchio Level 1 form 
/// </summary>
public sealed class Pinocchio : CardModel
{
    public override string Title => "Pinnochio";
    public override string Description => "On round start, create a Tall Tale in your hand.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Pinocchio.png";

    public override int BaseCost => 2;
    public override int BasePower => 3;
    public override int BasePlotArmor => 3;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    protected override Task SummonEffect()
    {
        OnRoundStart += CreateTallTaleInHand;
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        OnRoundStart -= CreateTallTaleInHand;
        return Task.CompletedTask;
    }

    private Task CreateTallTaleInHand()
    {
        CardModel card = CardFactory.Instance.CreateCard("TallTale", IsHidden, gameObject.transform, Owner, Board);
        Owner.handManager.AddCardToHandFromDeck(card);
        return Task.CompletedTask;
    }
}
