using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator Summoned(float dur = 0.5f, Action onComplete = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        yield return CoroutineUtils.Lerp(dur / 2, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.1f, 1.1f, 1.1f), t);
        });
        yield return CoroutineUtils.Lerp(dur / 2, (t) =>
        {
            rectTransform.localScale = Vector3.Lerp(new Vector3(1.1f, 1.1f, 1.1f), Vector3.one, t);
        });

        onComplete?.Invoke();
    }

    public IEnumerator Strike(float dur = 0.5f, Action onComplete = null)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        yield return CoroutineUtils.Lerp(dur / 4, (t) =>
        {
            rectTransform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0,0,45), t);
        });
        yield return CoroutineUtils.Lerp(dur / 4, (t) =>
        {
            rectTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 45), Quaternion.identity, t);
        });
        yield return CoroutineUtils.Lerp(dur / 4, (t) =>
        {
            rectTransform.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, 0, -45), t);
        });
        yield return CoroutineUtils.Lerp(dur / 4, (t) =>
        {
            rectTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, -45), Quaternion.identity, t);
        });

        onComplete?.Invoke();
    }
}
