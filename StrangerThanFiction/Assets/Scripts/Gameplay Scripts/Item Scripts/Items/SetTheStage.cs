using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTheStage : Item
{
    public SetTheStage(ItemInfo itemInfo, CardModel card) : base(itemInfo, card) { }

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
        for (int i = card.Owner.Deck.Count - 1; i >= 0; i--)
        {
            if (card.Owner.Deck[i].Type == CardType.Unit)
            {
                await card.Owner.Deck[i].GrantPower(1);
                break;
            }
        }
    }
}