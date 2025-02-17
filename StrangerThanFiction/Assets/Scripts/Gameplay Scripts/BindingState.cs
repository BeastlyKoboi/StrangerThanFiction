using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BindingState
{
    public BindingState(int bindingPower, int prevTotalBindingDamage, int currTotalBindingDamage)
    {
        this.bindingPower = bindingPower;
        this.prevBindingDamage = prevTotalBindingDamage;
        this.currTotalBindingDamage = currTotalBindingDamage;
    }

    public int bindingPower { get; set; }
    public int prevBindingDamage { get; set; }
    public int currTotalBindingDamage { get; set; }
}
