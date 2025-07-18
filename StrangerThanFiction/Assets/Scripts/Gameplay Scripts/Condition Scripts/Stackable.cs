using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stackable : Condition
{
    public List<string> stackableTitles;

    public Stackable(CardModel card, int amount, List<string> additionalStackTitles = null) : base(card, amount) 
    {
        this.stackableTitles = new List<string>();
        this.stackableTitles.Add(card.Title); // Add the card's own title to the stackable titles

        if (additionalStackTitles != null)
            this.stackableTitles.AddRange(additionalStackTitles);
    }

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
            if (stackableTitles.Contains(units[i].Title) && units[i] != card)
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
        if ((stackableTitles.Contains(unit.Title)) && unit != card && !unit.IsRemoved)
        {
            await card.GrantPower(unit.CurrentPower);
            await unit.Remove();
        }

    }

    private void AddStackableTitle(string newTitle)
    {
        stackableTitles.Add(newTitle);
    }

}
