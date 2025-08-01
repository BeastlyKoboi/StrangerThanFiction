using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoonCollection
{
    List<Boon> boons = new List<Boon>();

    public void AddBoon(Boon boon)
    {
        if (boon == null || boons.Contains(boon))
            return;
        boons.Add(boon);
    }

    public void RemoveBoon(Boon boon)
    {
        if (boon == null || !boons.Contains(boon))
            return;
        boons.Remove(boon);
    }

    public void EnterCombat(Player player)
    {
        foreach (Boon boon in boons)
        {
            boon.EnterCombat(player);
        }
    }
    public void ExitCombat()
    {
        foreach (Boon boon in boons)
        {
            boon.ExitCombat();
        }
    }



}
