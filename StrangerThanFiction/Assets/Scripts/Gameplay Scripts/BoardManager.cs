using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI.Table;
using static UnityEngine.UI.CanvasScaler;

/// <summary>
/// The BoardManager will be responsible for managing the game board, 
/// including the units and how they are interacted with.
/// </summary>
public class BoardManager : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;

    public UnitRow playerRow;

    public UnitRow enemyRow;

    public UniTask DeployUnit(CardModel unit, UnitRow row)
    {
        row.AddUnit(unit);

        unit.OnDestroy.AddListener(DestroyUnit);
        unit.OnRemove.AddListener(RemoveUnit);

        return UniTask.CompletedTask;
    }

    public void ReplaceUnit(CardModel oldUnit, CardModel newUnit)
    {
        newUnit.SelectedArea = oldUnit.SelectedArea;
        newUnit.SelectedArea.ReplaceUnit(oldUnit, newUnit);
    }

    /// <summary>
    /// Adds a unit to the board.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="row"></param>
    public UniTask SummonUnit(CardModel unit, UnitRow row)
    {
        return UniTask.CompletedTask;
    }

    private async UniTask DestroyUnit(CardModel unit)
    {
        await unit.Owner.UnitDestroyed(unit);
        unit.OnDestroy.RemoveListener(DestroyUnit);
        
        if (unit.SelectedArea != null)
            unit.SelectedArea.RemoveUnit(unit);
    }

    private UniTask RemoveUnit(CardModel unit)
    {
        if (unit.SelectedArea != null)
            unit.SelectedArea.RemoveUnit(unit);

        unit.OnRemove.RemoveListener(RemoveUnit);

        return UniTask.CompletedTask;
    }


    /// <summary>
    /// Checks if the pointer is above a valid placeable area for the player. 
    /// </summary>
    /// <param name="eventData"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    public bool CheckValidPlacement(PointerEventData eventData, CardModel card)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        for (int i = 0; i < raycastResults.Count; i++)
        {
            if (playerRow.gameObject == raycastResults[i].gameObject)
            {
                card.SelectedArea = playerRow;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Performs the round start events on all units on the board.
    /// This will start them, but will not wait for them to finish
    /// </summary>
    public async UniTask RoundStart(RoundStartState roundStartState)
    {
        await playerRow.ForEach(async unit => await unit.RoundStart(roundStartState));
        await enemyRow.ForEach(async unit => await unit.RoundStart(roundStartState));
    }

    /// <summary>
    /// Performs the round end events on all units on the board.
    /// This will start them, but will not wait for them to finish
    /// </summary>
    public async UniTask RoundEnd()
    {
        await playerRow.ForEach(async unit => await unit.RoundEnd());
        await enemyRow.ForEach(async unit => await unit.RoundEnd());
    }


    public async UniTask SetOnClickForPlayersUnits(Player player, Action<CardModel> action)
    {
        UnitRow unitRow = player == combatManager.player1 ? playerRow : enemyRow;

        await unitRow.ForEach(unit => { 
            unit.GetComponent<Clickable>().SetOnClickWithoutDrag(action);
            return UniTask.CompletedTask;
        });
    }

    public async UniTask SetOnClickForUnitRowsUnits(UnitRow specificRow, Action<CardModel> action)
    {
        await specificRow.ForEach(unit => {
            unit.GetComponent<Clickable>().SetOnClickWithoutDrag(action);
            return UniTask.CompletedTask;
        });
    }

    /// <summary>
    /// Returns the strongest unit of a specific player on the board.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public CardModel GetStrongestUnit(Player player)
    {
        UnitRow unitRow = player == combatManager.player1 ? playerRow : enemyRow;

        return unitRow.GetStrongestUnit();
    }

    /// <summary>
    /// Returns the weakest unit of a specific player on the board.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public CardModel GetWeakestUnit(Player player)
    {
        UnitRow unitRow = player == combatManager.player1 ? playerRow : enemyRow;

        return unitRow.GetWeakestUnit();
    }

    public CardModel GetRandomUnit(Player player)
    {
        CardModel[] units = GetUnits(player);
        return units.Length > 0? units[UnityEngine.Random.Range(0, units.Length)]: null;
    }

    /// <summary>
    /// Returns the total power of a specific player.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public int GetTotalPower(Player player) => (player == combatManager.player1 ? playerRow : enemyRow).GetTotalPower();

    public UnitRow GetEnemyRow() => enemyRow;

    public UnitRow GetValidRow(Player player)
    {
        UnitRow unitRow = player == combatManager.player1 ? playerRow : enemyRow;

        if (unitRow.GetIsFull())
            return null;

        return unitRow;
    }

    public CardModel[] GetUnits(Player player) => (player == combatManager.player1 ? playerRow : enemyRow).GetUnits();


    public int CalculateCurrentBindingDamage()
    {
        int playerPower = GetTotalPower(combatManager.player1);
        int enemyPower = GetTotalPower(combatManager.player2);

        return playerPower - enemyPower / 2;
    }

}
