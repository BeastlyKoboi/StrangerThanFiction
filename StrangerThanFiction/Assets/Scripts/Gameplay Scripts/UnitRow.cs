using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class UnitRow : MonoBehaviour
{
    public List<CardModel> units;
    public List<RectTransform> unitRects;
    delegate bool filterDelegate(CardModel unit);

    public void AddUnit(CardModel newUnit)
    {
        newUnit.transform.SetParent(transform);

        units.Add(newUnit);
        unitRects.Add(newUnit.GetComponent<RectTransform>());

        UpdateUnitPositions();
    }

    public void RemoveUnit(CardModel unitToRemove)
    {
        // test
        int index = units.IndexOf(unitToRemove);
        if (index != -1)
        {
            units.RemoveAt(index);
            unitRects.RemoveAt(index);
            UpdateUnitPositions();
        }
    }

    public void UpdateUnitPositions()
    {
        if (units.Count == 0) return;
        float unitWidth = units[0].unitView.transform.localScale.x * units[0].unitView.GetComponent<RectTransform>().rect.width;
        // Find out why next line does not work
        //float unitWidth = units[0].unitView.transform.localScale.x * unitRects[0].rect.width;
        float filledRowWidth = units.Count * unitWidth;
        float startingXPos = -filledRowWidth / 2 + unitWidth / 2;

        for (int i = 0; i < units.Count; i++)
        {
            unitRects[i].anchoredPosition = new Vector2(startingXPos + i * unitWidth, 0);
            unitRects[i].rotation = Quaternion.identity;
        }

    }
    
    public async Task ForEach(Func<CardModel, Task> func)
    {
        for (int i = 0; i < units.Count; i++)
        {
            await func(units[i]);
        }
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

        units.ForEach(unit => { total += unit.CurrentPower; });

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
        return units.ToArray();
    }
}
