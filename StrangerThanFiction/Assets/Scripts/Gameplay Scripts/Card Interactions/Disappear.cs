using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    public IEnumerator AnimateRemove(float delay = 0.5f, float duration = 0.25f, Action onComplete = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        Vector3 leftPos = new Vector3(startPos.x - 25, startPos.y, startPos.z);
        Vector3 rightPos = new Vector3(startPos.x + 25, startPos.y, startPos.z);

        yield return new WaitForSeconds(delay);

        // Maybe a shiver effect or something

        // for now an interesting rotation
        yield return CoroutineUtils.Lerp(duration / 6 / 2, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, leftPos, t);
        });
        yield return CoroutineUtils.Lerp(duration / 6, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(leftPos, rightPos, t);
        });
        yield return CoroutineUtils.Lerp(duration / 6, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rightPos, leftPos, t);
        });
        yield return CoroutineUtils.Lerp(duration / 6, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(leftPos, rightPos, t);
        });
        yield return CoroutineUtils.Lerp(duration / 6, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rightPos, leftPos, t);
        });
        yield return CoroutineUtils.Lerp(duration / 6, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(leftPos, rightPos, t);
        });
        yield return CoroutineUtils.Lerp(duration / 6 / 2, (t) =>
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rightPos, startPos, t);
        });

        onComplete?.Invoke();
    }
}
