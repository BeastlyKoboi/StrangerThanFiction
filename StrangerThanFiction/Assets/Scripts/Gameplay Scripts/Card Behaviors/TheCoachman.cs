using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TheCoachman : CardModel
{
    protected override async Task SummonEffect()
    {
        await SummonDonkey();
        Owner.OnRoundStart += SummonDonkey;
    }

    protected override Task RemoveEffect(CardModel card)
    {
        Owner.OnRoundStart -= SummonDonkey;
        return Task.CompletedTask;
    }

    private async Task SummonDonkey()
    {
        CardModel donkey = CardFactory.Instance.CreateCard("Donkey", true, transform, Owner, Board, Title);

        await donkey.Summon();
    }
}
