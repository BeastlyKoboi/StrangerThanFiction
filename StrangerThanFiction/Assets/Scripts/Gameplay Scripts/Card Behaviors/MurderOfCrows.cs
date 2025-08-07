using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class MurderOfCrows : CardModel
{
    protected override async void Awake()
    {
        base.Awake();

        await ApplyCondition(new Stackable(this, 0, new List<string>() { "Crow" }));
    }

    protected override UniTask DeployEffect(DeployState deployState = null)
    {
        Owner.OnUnitDestroyed.AddListener(DamageAllEnemies);
        return base.DeployEffect(deployState);
    }

    protected override UniTask RemoveEffect(CardModel card)
    {
        Owner.OnUnitDestroyed.RemoveListener(DamageAllEnemies);
        return base.RemoveEffect(card);
    }

    private async UniTask DamageAllEnemies(CardModel unit)
    {
        CardModel[] enemies = Board.GetUnits(Owner.enemyPlayer);

        for (int i = 0; i < enemies.Length; i++)
        {
            await enemies[i].TakeDamage(new DamageData(1, this));
        }
    }
}
