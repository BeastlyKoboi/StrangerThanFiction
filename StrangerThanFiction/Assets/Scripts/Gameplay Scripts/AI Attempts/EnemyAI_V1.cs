using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// The first placeholder attempt at an enemy AI.
/// </summary>
public class EnemyAI_V1 : MonoBehaviour
{
    public Player myPlayer;

    public Player opponent; // human player

    // Start is called before the first frame update
    void Start()
    {
        myPlayer = GetComponent<Player>();
        myPlayer.OnMyTurnStart += PlayTurn;
    }

    /// <summary>
    /// The enemy AI will play the first card in its hand that is playable.
    /// </summary>
    public void PlayTurn()
    {
        CardPlayState playState = new CardPlayState(myPlayer.handManager.Hand[0]);

        for (int cardIndex = 0; cardIndex < myPlayer.handManager.Hand.Count; cardIndex++)
        {
            if (myPlayer.handManager.Hand[cardIndex].Playable)  
            {
                playState.card = myPlayer.handManager.Hand[cardIndex];
                break;
            }
        }

        Debug.Log("Playing " + playState.card.Title);
        Debug.Log("Hand count " + myPlayer.handManager.Hand.Count);

        if (playState.card.PlayRequirements.AllyUnitTargets != 0)
        {
            for (int i = 0; i < playState.card.PlayRequirements.AllyUnitTargets; i++)
            {
                CardModel target;
                do
                {
                    target = myPlayer.board.GetRandomUnit(myPlayer);
                } while (playState.allyUnitTargets.Contains(target));
                playState.allyUnitTargets.Add(target);
            }
        }
        if (playState.card.PlayRequirements.EnemyUnitTargets != 0)
        {
            for (int i = 0; i < playState.card.PlayRequirements.EnemyUnitTargets; i++)
            {
                CardModel target;
                do
                {
                    target = opponent.board.GetRandomUnit(opponent);
                } while (playState.enemyUnitTargets.Contains(target));
                playState.enemyUnitTargets.Add(target);
            }
        }
        if (playState.card.PlayRequirements.AllyCardTargets != 0)
            ;
        if (playState.card.PlayRequirements.EnemyCardTargets != 0)
            ;

        if (playState.card.Type == CardType.Unit)
        {
            playState.card.SelectedArea = playState.card.Board.GetRandomEnemyRow();
        }

        myPlayer.handManager.SetCardPlayState(playState.card,
            allyUnitTargets: playState.allyUnitTargets,
            enemyUnitTargets: playState.enemyUnitTargets, 
            allyCardTargets: playState.allyCardTargets,
            enemyCardTargets: playState.enemyCardTargets);

    }
}
