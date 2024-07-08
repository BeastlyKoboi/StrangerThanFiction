using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The BoardManager will be responsible for managing the game board, 
/// including the units and how they are interacted with.
/// </summary>
public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public UnitRow player1FrontRow;
    public UnitRow player1BackRow;

    public UnitRow player2FrontRow;
    public UnitRow player2BackRow;

    /// <summary>
    /// Adds a unit to the board.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="row"></param>
    public void SummonUnit(CardModel unit, UnitRow row)
    {
        unit.IsHidden = false;

        row.AddUnit(unit);

        unit.Owner.UnitSummoned(unit);
    }

    /// <summary>
    /// Checks if the pointer is above a valid placeable area for the player. 
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool CheckPointerAboveArea(PointerEventData eventData, CardModel card)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        UnitRow frontline = card.Owner == gameManager.player1 ? player1FrontRow : player2FrontRow;
        UnitRow backline = card.Owner == gameManager.player1 ? player1BackRow : player2BackRow;

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (frontline.gameObject == raycastResults[i].gameObject || backline.gameObject == raycastResults[i].gameObject)
            {
                card.SelectedArea = raycastResults[i].gameObject == frontline.gameObject ? frontline : backline;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Performs the round start events on all units on the board.
    /// </summary>
    public void RoundStart()
    {
        player1FrontRow.ForEach(unit => unit.RoundStart());
        player1BackRow.ForEach(unit => unit.RoundStart());
        player2FrontRow.ForEach(unit => unit.RoundStart());
        player2BackRow.ForEach(unit => unit.RoundStart());
    }

    /// <summary>
    /// Performs the round end events on all units on the board.
    /// </summary>
    public void RoundEnd()
    {
        player1FrontRow.ForEach(unit => unit.RoundEnd());
        player1BackRow.ForEach(unit => unit.RoundEnd());
        player2FrontRow.ForEach(unit => unit.RoundEnd());
        player2BackRow.ForEach(unit => unit.RoundEnd());
    }

    /// <summary>
    /// Returns the strongest unit of a specific player on the board.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public CardModel GetStrongestUnit(Player player)
    {
        UnitRow frontline = player == gameManager.player1 ? player1FrontRow : player2FrontRow;
        UnitRow backline = player == gameManager.player1 ? player1BackRow : player2BackRow;

        CardModel frontStrongest = frontline.GetStrongestUnit();
        CardModel backStrongest = backline.GetStrongestUnit();

        if (frontStrongest == null) return backStrongest;
        if (backStrongest == null) return frontStrongest;

        if (frontStrongest.CurrentPower > backStrongest.CurrentPower)
            return frontStrongest;
        else if (frontStrongest.CurrentPower == backStrongest.CurrentPower &&
            frontStrongest.CurrentPlotArmor >= backStrongest.CurrentPlotArmor)
            return frontStrongest;

        return backStrongest;
    }

    /// <summary>
    /// Returns the weakest unit of a specific player on the board.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public CardModel GetWeakestUnit(Player player)
    {
        UnitRow frontline = player == gameManager.player1 ? player1FrontRow : player2FrontRow;
        UnitRow backline = player == gameManager.player1 ? player1BackRow : player2BackRow;

        CardModel frontWeakest = frontline.GetWeakestUnit();
        CardModel backWeakest = backline.GetWeakestUnit();

        if (frontWeakest == null) return backWeakest;
        if (backWeakest == null) return frontWeakest;

        if (frontWeakest.CurrentPower < backWeakest.CurrentPower)
            return frontWeakest;
        else if (frontWeakest.CurrentPower == backWeakest.CurrentPower &&
            frontWeakest.CurrentPlotArmor <= backWeakest.CurrentPlotArmor)
            return frontWeakest;

        return backWeakest;
    }

    /// <summary>
    /// Returns the total power of the front row of a specific player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public int GetTotalFrontPower(Player player)
    {
        return (player == gameManager.player1 ? player1FrontRow : player2FrontRow).GetTotalPower();
    }

    /// <summary>
    /// Returns the total power of the back row of a specific player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public int GetTotalBackPower(Player player)
    {
        return (player == gameManager.player1 ? player1BackRow : player2BackRow).GetTotalPower();
    }

    /// <summary>
    /// Returns the total power of a specific player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public int GetTotalPower(Player player)
    {
        return GetTotalFrontPower(player) + GetTotalBackPower(player);
    }

    /// <summary>
    /// Returns a random enemy UnitRow
    /// </summary>
    /// <returns></returns>
    public UnitRow GetRandomEnemyRow()
    {
        return (Random.value > 0.5) ? player2BackRow : player2FrontRow;
    }

    /// <summary>
    /// Returns the units of a specific player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public CardModel[] GetUnits(Player player)
    {
        UnitRow frontline = player == gameManager.player1 ? player1FrontRow : player2FrontRow;
        UnitRow backline = player == gameManager.player1 ? player1BackRow : player2BackRow;

        List<CardModel> units = new List<CardModel>();
        units.AddRange(frontline.GetUnits());
        units.AddRange(backline.GetUnits());

        return units.ToArray();
    }
}
