using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStrikeState 
{
    public CardModel striker;
    public IDamagable target;

    public UnitStrikeState(CardModel striker = null, IDamagable target = null)
    {
        this.striker = striker;
        this.target = target;
    }
}
