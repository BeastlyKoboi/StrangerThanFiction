using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SetTheStageIII : Item
{
    public SetTheStageIII(ItemInfo itemInfo, CardModel card) : base(itemInfo, card)
    {

    }

    public override UniTask OnAdd()
    {
        card.OnPlay.AddListener(OnPlay);
        return base.OnAdd();
    }

    public override UniTask OnRemove()
    {
        card.OnPlay.RemoveListener(OnPlay);
        return base.OnRemove();
    }

    public async UniTask OnPlay(CardPlayState cardPlayState)
    {
        List<CardModel> unitsInDeck = new List<CardModel>();
        for (int i = card.Owner.Deck.Count - 1; i >= 0; i--)
        {
            if (card.Owner.Deck[i].Type == CardType.Unit)
            {
                unitsInDeck.Add(card.Owner.Deck[i]);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            await unitsInDeck[Random.Range(0, unitsInDeck.Count)].GrantPower(3);
        }
    }
}
