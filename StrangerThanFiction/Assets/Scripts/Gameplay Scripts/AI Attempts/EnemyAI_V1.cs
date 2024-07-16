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
        CardModel cardToPlay = myPlayer.handManager.Hand[0];

        for (int cardIndex = 0; cardIndex < myPlayer.handManager.Hand.Count; cardIndex++)
        {
            if (myPlayer.handManager.Hand[cardIndex].Playable)
            {
                cardToPlay = myPlayer.handManager.Hand[cardIndex];
                break;
            }
        }

        if (cardToPlay.Type == CardType.Unit)
        {
            cardToPlay.SelectedArea = cardToPlay.Board.GetRandomEnemyRow();
        }

        myPlayer.handManager.playedCard = cardToPlay;

    }
}
