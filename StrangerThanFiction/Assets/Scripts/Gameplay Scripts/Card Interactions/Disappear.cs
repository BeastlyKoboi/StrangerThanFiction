using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{

    private void Start()
    {
        
    }

    public IEnumerator AnimateDiscard(Action onComplete = null) {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        yield return CoroutineUtils.Lerp(0.25f, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.8f, 0.8f, 0.8f), t);
        });
        yield return CoroutineUtils.Lerp(0.25f, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.8f, 0.8f), Vector3.one, t);
        });


        yield return CoroutineUtils.Lerp(0.5f, (t) =>
        {           
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, Vector3.zero, t);
            rectTransform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
        });

        onComplete?.Invoke();
    }

    public IEnumerator AnimateDestroy(Action onComplete = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.localRotation;

        yield return new WaitForSeconds(0.5f);

        yield return CoroutineUtils.Lerp(0.25f, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            rectTransform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
        });

        onComplete?.Invoke();
    }
}
