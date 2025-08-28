using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionResult
{
    public bool AllowSelection;
    public bool ConfirmInteractable;
    public SelectionResult(bool allow, bool interactable)
    {
        AllowSelection = allow;
        ConfirmInteractable = interactable;
    }
}