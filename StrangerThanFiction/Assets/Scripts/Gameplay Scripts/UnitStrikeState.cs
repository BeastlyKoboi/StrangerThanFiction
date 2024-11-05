using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStrikeState 
{
    public CardModel striker;
    public CardModel target;

    public UnitStrikeState(CardModel striker = null, CardModel target = null)
    {
        this.striker = striker;
        this.target = target;
    }
}
