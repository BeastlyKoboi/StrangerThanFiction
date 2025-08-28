using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateUIState
{
    public List<EncounterOption> options = new List<EncounterOption>();
    public Dictionary<EncounterOptionType, GameObject> zones = new Dictionary<EncounterOptionType, GameObject>(); // For dynamic zone assignment
    public bool showPrices = false;
    public bool showNewCardItems = false;
}

