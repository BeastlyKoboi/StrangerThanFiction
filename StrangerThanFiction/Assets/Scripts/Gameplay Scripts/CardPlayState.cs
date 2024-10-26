using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPlayState
{
    public CardModel card;

    public List<CardModel> allyUnitTargets;
    public List<CardModel> enemyUnitTargets;

    public List<CardModel> allyCardTargets;
    public List<CardModel> enemyCardTargets;

    public CardModel replacedCard;

    public CardPlayState(
        CardModel card,
        List<CardModel> allyUnitTargets = null,
        List<CardModel> enemyUnitTargets = null,
        List<CardModel> allyCardTargets = null,
        List<CardModel> enemyCardTargets = null,
        CardModel replacedCard = null)
    {
        this.card = card;

        if (allyUnitTargets != null)
            this.allyUnitTargets = allyUnitTargets;
        else
            this.allyUnitTargets = new List<CardModel>();

        if (enemyUnitTargets != null)
            this.enemyUnitTargets = enemyUnitTargets;
        else
            this.enemyUnitTargets = new List<CardModel>();

        if (allyCardTargets != null)
            this.allyCardTargets = allyCardTargets;
        else
            this.allyCardTargets = new List<CardModel>();

        if (enemyCardTargets != null)
            this.enemyCardTargets = enemyCardTargets;
        else
            this.enemyCardTargets = new List<CardModel>();

        this.replacedCard = replacedCard;
    }
}
