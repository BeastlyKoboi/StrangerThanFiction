using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MurderOfCrows : CardModel
{
    public override uint Id => 14;

    protected override async Task SummonEffect()
    {
        CardModel[] enemies = Board.GetUnits(Owner.enemyPlayer);

        for (int i = 0; i < enemies.Length; i++)
        {
            await enemies[i].TakeDamage(1);
        }
    }
}
