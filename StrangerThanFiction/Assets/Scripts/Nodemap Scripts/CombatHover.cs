using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    [SerializeField] private BattleNode battleNode;
    [SerializeField] private GameObject popupWindow;
    private CanvasGroup popupWindowCanvasGroup;
    private RectTransform popupWindowRect;
    [SerializeField] private TextMeshProUGUI bindingValue;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        popupWindow = GameObject.Find("CombatPreview").gameObject;
        popupWindowRect = popupWindow.GetComponent<RectTransform>();
        popupWindowCanvasGroup = popupWindow.GetComponent<CanvasGroup>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (popupWindow != null)
        {
            SetBinding(battleNode.GetBinding());

            // Set the position of the popup window to the position of the node
            popupWindowRect.position = rectTransform.position;
            popupWindowRect.position += new Vector3(0, rectTransform.rect.height / 2 + popupWindowRect.rect.height / 2, 0);
            popupWindowCanvasGroup.alpha = 1;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (popupWindow != null)
        {
            popupWindowCanvasGroup.alpha = 0;
        }
    }

    public void SetPopupWindow(GameObject window)
    {
        popupWindow = window;
    }

    public void SetBattleNode(BattleNode node)
    {
        battleNode = node;
    }

    private void SetBinding(int newBinding)
    {
        if (bindingValue == null)
        {
            bindingValue = popupWindow.transform.Find("BindingValue").GetComponent<TextMeshProUGUI>();
        }
        bindingValue.text = newBinding.ToString();
    }

    public void SetPowers()
    {

    }
}
