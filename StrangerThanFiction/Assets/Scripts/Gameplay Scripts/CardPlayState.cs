using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayState
{
    public CardModel card;
    public Player player;
    public Player enemy;

    public List<CardModel> allyUnitTargets;
    public List<CardModel> enemyUnitTargets;

    public List<CardModel> allyCardTargets;
    public List<CardModel> enemyCardTargets;

    public CardPlayState(
        CardModel card,
        Player player = null, 
        Player enemy = null, 
        List<CardModel> allyUnitTargets = null, 
        List<CardModel> enemyUnitTargets = null, 
        List<CardModel> allyCardTargets = null, 
        List<CardModel> enemyCardTargets = null)
    {
        this.card = card;
        this.player = player;
        this.enemy = enemy;
        this.allyUnitTargets = allyUnitTargets;
        this.enemyUnitTargets = enemyUnitTargets;
        this.allyCardTargets = allyCardTargets;
        this.enemyCardTargets = enemyCardTargets;
    }
}
