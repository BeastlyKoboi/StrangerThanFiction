using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BindingHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject popupWindow;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (popupWindow != null) 
        {
            popupWindow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (popupWindow != null)
        {
            popupWindow.SetActive(false);
        }
    }

}
