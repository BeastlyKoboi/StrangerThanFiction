using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardList", menuName = "Variables/CardList")]
public class CardList : ScriptableObject
{
    public CardInfo[] cards;
}