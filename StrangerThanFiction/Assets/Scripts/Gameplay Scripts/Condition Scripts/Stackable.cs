using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stackable : Condition
{
    public Stackable(CardModel card, int amount) : base(card, amount) { }

    public override UniTask OnAdd()
    {
        card.OnDeploy.AddListener(AddAura);

        return base.OnAdd();
    }

    public override UniTask OnRemove()
    {
        card.OnDeploy.RemoveListener(AddAura);

        return base.OnRemove();
    }

    private async UniTask AddAura(DeployState deployState)
    {
        bool otherCopyExists = false;
        CardModel[] units = card.Board.GetUnits(card.Owner);
        for (int i = 0; i < units.Length; i++)
        {
            if (units[i].Title == card.Title && units[i] != card)
            {
                otherCopyExists = true;
                break;
            }
        }
        if (otherCopyExists)
        {
            return;
        }
        else
        {
            card.Owner.OnUnitSummoned.AddListener(StackableAura);
            card.OnDestroy.AddListener(RemoveAura);
        }
    }

    private async UniTask RemoveAura(CardModel cardModel)
    {
        card.Owner.OnUnitSummoned.RemoveListener(StackableAura);
        card.OnDestroy.RemoveListener(RemoveAura);
    }

    private async UniTask StackableAura(CardModel unit)
    {
        // if self do nothing
        if (unit == card) return;

        // The aura is: whenever a copy of this card is summoned, grant this card it's power and remove the copy. 
        if (unit.Title == card.Title && unit != card)
        {
            await card.GrantPower(unit.CurrentPower);
            await unit.Remove();
        }

    }

}
