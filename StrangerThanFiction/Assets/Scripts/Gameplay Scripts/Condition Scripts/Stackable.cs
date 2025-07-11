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
        card.OnDestroy.AddListener(RemoveAura);

        return base.OnAdd();
    }

    public override UniTask OnTrigger()
    {
        return base.OnTrigger();
    }

    public override UniTask OnSurplus(Condition surplus)
    {
        return UniTask.CompletedTask;
    }

    public override UniTask OnRemove()
    {
        card.OnDeploy.RemoveListener(AddAura);
        card.OnDestroy.RemoveListener(RemoveAura);

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

        }
    }

    private async UniTask RemoveAura(CardModel cardModel)
    {

    }

    private async UniTask StackableAura(CardModel unit)
    {
        // if self do nothing
        if (unit == card) return;

        // if other copy is already on the board return
        

        

        // if not make sure to add the aura to the card

        // The aura is: whenever a copy of this card is summoned, grant this card it's power and remove the copy. 

        await card.GrantPower(card.CurrentPower);




    }

}
