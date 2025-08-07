using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BigRedCap : CardModel
{
    // Stackable. Little Red Caps stack with me. When I gain power, grant the strongest enemy Poisoned equal to power gained.
    protected async override void Awake()
    {
        base.Awake();

        await ApplyCondition(new Stackable(this, 0, new List<string>() { typeof(LittleRedCap).ToString() }));
    }

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        OnGrantPower.AddListener(PoisonStrongestEnemy);
        return UniTask.CompletedTask;
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        OnGrantPower.RemoveListener(PoisonStrongestEnemy);
        return UniTask.CompletedTask;
    }

    private async UniTask PoisonStrongestEnemy(int powerGranted)
    {
        CardModel strongestUnit = Owner.board.GetStrongestUnit(Owner.enemyPlayer);
        if (strongestUnit == null) return;
        await strongestUnit.ApplyCondition(new Poisoned(strongestUnit, powerGranted));
    }
}
