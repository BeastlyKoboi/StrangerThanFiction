using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thorns : Condition
{
    public Thorns(CardModel card, int amount) : base(card, amount)
    {
    }

    public override UniTask OnAdd()
    {
        card.OnSurviveDamage.AddListener(ActualTrigger);
        return UniTask.CompletedTask;
    }   
    public override async UniTask OnTrigger() 
    {
        CardModel thornedEnemy = card.Board.GetRandomUnit(card.Owner.enemyPlayer);
        await thornedEnemy.TakeDamage(new DamageData(amount, this));
        amount -= 1;
        if (amount == 0)
        {
            await card.RemoveCondition(Name);
        }
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        if (surplus is Thorns thornsSurplus)
        {
            amount += thornsSurplus.amount;
        }
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.OnSurviveDamage.RemoveListener(ActualTrigger);
        return UniTask.CompletedTask;
    }

    private async UniTask ActualTrigger(DamageData damageData)
    {
        if (damageData.source is Thorns) return;

        await OnTrigger();
    }
}
