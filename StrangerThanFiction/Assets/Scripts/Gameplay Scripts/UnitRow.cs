using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UnitRow : MonoBehaviour
{
    [SerializeField] private UnitSlot[] unitSlots = new UnitSlot[7];

    //private List<CardModel> units;
    //private List<RectTransform> unitRects;
    //private List<CardView> unitViews;

    private int maxUnits = 7;

    private void Awake()
    {

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (unitSlots[i] == null) continue;
            unitSlots[i].SlotIndex = i;
        }
    }

    public void AddUnit(CardModel newUnit)
    {
        if (newUnit.SelectedAreaSlot == null)
        {
            newUnit.SelectedAreaSlot = unitSlots.FirstOrDefault((unitSlot) => unitSlot.IsEmpty() );
        }

        newUnit.SelectedAreaSlot.SetNewUnit(newUnit, newUnit.cardView);
    }

    public void RemoveUnit(CardModel unitToRemove)
    {
        UnitSlot slot = unitSlots.FirstOrDefault((unitSlot) => unitSlot.Unit == unitToRemove);
        if (slot != null)
        {
            unitSlots[slot.SlotIndex].RemoveUnit();
        }
    }

    public void ReplaceUnit(CardModel oldUnit, CardModel newUnit)
    {
        UnitSlot slot = unitSlots.FirstOrDefault((unitSlot) => unitSlot.Unit == oldUnit);
        if (slot != null)
        {
            unitSlots[slot.SlotIndex].SetNewUnit(newUnit, newUnit.cardView);
        }
    }

    //public void UpdateUnitPositions()
    //{
    //    if (units.Count == 0) return;
    //    float unitWidth = unitViews[0].unitTransform.transform.localScale.x * unitViews[0].unitTransform.GetComponent<RectTransform>().rect.width;
    //    // Find out why next line does not work
    //    //float unitWidth = units[0].unitView.transform.localScale.x * unitRects[0].rect.width;
    //    float filledRowWidth = units.Count * unitWidth;
    //    float startingXPos = -filledRowWidth / 2 + unitWidth / 2;

    //    for (int i = 0; i < units.Count; i++)
    //    {
    //        unitRects[i].anchoredPosition = new Vector2(startingXPos + i * unitWidth, 0);
    //        unitRects[i].rotation = Quaternion.identity;
    //    }

    //}
    
    public async UniTask ForEach(Func<CardModel, UniTask> func)
    {
        // Units are removing themselves during the loop, so we need to copy the list
        List<CardModel> units = new List<CardModel>();
        CardModel[] unitsArr = null;

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (!unitSlots[i].IsEmpty())
                units.Add(unitSlots[i].Unit);
        }

        unitsArr = units.ToArray();

        for (int i = 0; i < unitsArr.Length; i++)
        {
            await func(unitsArr[i]);
        }
    }

    public bool GetIsFull()
    {
        return GetUnits().Length == unitSlots.Length;
    }

    // ----------------------------------------------------------------------------
    // Methods used to query specific units 
    // ----------------------------------------------------------------------------

    /// <summary>
    /// Returns the strongest unit in this unit row, by power then plot armor.
    /// </summary>
    /// <returns></returns>
    public CardModel GetStrongestUnit()
    {
        List<CardModel> units = GetUnits().ToList();
        if (units.Count == 0) return null;

        CardModel[] unitsArr = units.ToArray();
        CardModel strongest = unitsArr[0];

        for (int i = 0; i < unitsArr.Length; i++)
        {
            if (strongest.CurrentPower < unitsArr[i].CurrentPower)
                strongest = unitsArr[i];
            else if (strongest.CurrentPower == unitsArr[i].CurrentPower &&
                strongest.CurrentPlotArmor < unitsArr[i].CurrentPlotArmor)
                strongest = unitsArr[i];
        }
        return strongest;
    }

    /// <summary>
    /// Returns the weakest unit in this unit row, by power then plot armor.
    /// </summary>
    /// <returns></returns>
    public CardModel GetWeakestUnit()
    {
        List<CardModel> units = GetUnits().ToList();
        if (units.Count == 0) return null;

        CardModel[] unitsArr = units.ToArray();
        CardModel weakest = unitsArr[0];

        for (int i = 0; i < unitsArr.Length; i++)
        {
            if (weakest.CurrentPower > unitsArr[i].CurrentPower)
                weakest = unitsArr[i];
            else if (weakest.CurrentPower == unitsArr[i].CurrentPower &&
                weakest.CurrentPlotArmor > unitsArr[i].CurrentPlotArmor)
                weakest = unitsArr[i];
        }

        //return units
        //    .OrderBy(u => u.CurrentPower)
        //    .ThenBy(u => u.CurrentPlotArmor)
        //    .FirstOrDefault();

        return weakest;
    }

    /// <summary>
    /// Calculates and returns the total power of units in this row. 
    /// </summary>
    /// <returns></returns>
    public int GetTotalPower()
    {
        int total = 0;

        GetUnits().ToList().ForEach(unit => { total += unit.CurrentPower; });

        // Other option
        // return units.Sum(unit => unit.CurrentPower);

        return total;
    }

    /// <summary>
    /// Returns an array of the units in this row.
    /// </summary>
    /// <returns></returns>
    public CardModel[] GetUnits()
    {
        List<CardModel> units = new List<CardModel>();

        for (int i = 0; i < unitSlots.Length; i++)
        {
            if (!unitSlots[i].IsEmpty())
                units.Add(unitSlots[i].Unit);
        }

        return units.ToArray();
    }

    public UnitSlot[] GetUnitSlots() => unitSlots;
    public GameObject[] GetUnitSlotObjs()
    {
        List<GameObject> unitSlotObjs = new List<GameObject>();

        for (int i = 0; i < unitSlots.Length; i++)
        {
            unitSlotObjs.Add(unitSlots[i].gameObject);
        }

        return unitSlotObjs.ToArray();
    }
}
