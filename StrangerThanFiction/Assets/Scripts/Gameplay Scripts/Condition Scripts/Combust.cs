using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Combust : Condition
{
    public override uint Id => 1;
    public Combust(CardModel card, int amount) : base(card, amount) { }
}
