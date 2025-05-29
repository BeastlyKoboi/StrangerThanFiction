using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageTab : MonoBehaviour
{
    public void OnTabClicked()
    {
       transform.SetAsLastSibling();
    }
}
