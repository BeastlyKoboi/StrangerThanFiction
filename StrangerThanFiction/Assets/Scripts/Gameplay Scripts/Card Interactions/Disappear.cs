using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{

    private void Start()
    {
        
    }

    public IEnumerator AnimateDiscard(float pulseDur = 0.5f, float discardDur = 0.5f, Action onComplete = null) {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        yield return CoroutineUtils.Lerp(pulseDur/2, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.8f, 0.8f, 0.8f), t);
        });
        yield return CoroutineUtils.Lerp(pulseDur/2, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 0.8f), Vector3.one, t);
        });

        yield return CoroutineUtils.Lerp(discardDur, (t) =>
        {           
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, Vector3.zero, t);
            rectTransform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
        });

        onComplete?.Invoke();
    }

    public IEnumerator AnimateDestroy(float delay = 0.5f, float duration = 0.25f, Action onComplete = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        yield return new WaitForSeconds(delay);

        yield return CoroutineUtils.Lerp(duration, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            rectTransform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
        });

        onComplete?.Invoke();
    }
}
