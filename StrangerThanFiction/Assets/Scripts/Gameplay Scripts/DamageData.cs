using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    public int damage;
    public IDamageSource source;
    public bool ignorePlotArmor;
    public bool ignoreResistances;

    public DamageData(int damage = 0, IDamageSource source = null, bool ignorePlotArmor = false, bool ignoreResistances = false)
    {
        this.damage = damage;
        this.source = source;
        this.ignorePlotArmor = ignorePlotArmor;
        this.ignoreResistances = ignoreResistances;
    }
}
