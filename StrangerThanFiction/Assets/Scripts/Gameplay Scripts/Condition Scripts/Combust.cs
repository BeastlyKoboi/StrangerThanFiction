using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Combust : Condition
{
    public static new string StaticName => typeof(Combust).Name;
    public override string Name => StaticName;
    public override string Description { get; } = "Card is destroyed when played.";

    public Combust(CardModel card, int amount) : base(card, amount) { }
}
