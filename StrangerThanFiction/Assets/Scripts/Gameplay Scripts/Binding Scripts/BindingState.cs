using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindingState
{

    public BindingState(BattleNodeData battleNodeData, int bindingPower, int prevTotalBindingDamage, int currTotalBindingDamage, BoonCollection boons = null)
    {
        this.BattleNodeData = battleNodeData;
        this.bindingPower = bindingPower;
        this.prevBindingDamage = prevTotalBindingDamage;
        this.currTotalBindingDamage = currTotalBindingDamage;
        this.Boons = boons ?? new BoonCollection();
    }

    public BattleNodeData BattleNodeData { get; set; }

    public int bindingPower { get; set; }
    public int prevBindingDamage { get; set; }
    public int currTotalBindingDamage { get; set; }

    public BoonCollection Boons { get; set; }
}
