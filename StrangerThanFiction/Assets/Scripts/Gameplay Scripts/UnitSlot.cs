using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour, ISelectable
{
    public int SlotIndex { get; set; }
    [SerializeField] private CardModel unit;
    [SerializeField] private CardView unitCardView;

    [SerializeField] private Image selectableIcon;
    [SerializeField] private Clickable clickable;

    public bool IsEmpty() => unit == null;

    public CardModel Unit => unit;

    private Coroutine pulse;

    public void SetNewUnit(CardModel unit, CardView cardView)
    {
        this.unit = unit;
        unitCardView = cardView;

        unit.transform.SetParent(transform);
        unit.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    public void RemoveUnit()
    {
        this.unit = null;
        unitCardView = null;
    }

    public void ToggleTargetable(bool targetable)
    {
        selectableIcon.gameObject.SetActive(targetable);

        if (targetable && pulse == null)
            pulse = StartCoroutine(Pulse(shouldLoop: true));
        else
        {
            StopCoroutine(pulse);
            pulse = null;
        }
    }

    public void SetOnLeftClick(Action<ISelectable> action)
    {
        clickable.SetOnLeftClick(action);
    }

    public IEnumerator Pulse(float dur = 1.0f, Action onComplete = null, bool shouldLoop = false)
    {
        RectTransform rectTransform = selectableIcon.gameObject.GetComponent<RectTransform>();

        yield return CoroutineUtils.Lerp(dur / 2, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(.8f, .8f, .8f), t);
        });
        yield return CoroutineUtils.Lerp(dur / 2, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(new Vector3(.8f, .8f, .8f), Vector3.one, t);
        });

        onComplete?.Invoke();

        if (shouldLoop)
            pulse = StartCoroutine(Pulse(dur, onComplete, shouldLoop));
    }
}
