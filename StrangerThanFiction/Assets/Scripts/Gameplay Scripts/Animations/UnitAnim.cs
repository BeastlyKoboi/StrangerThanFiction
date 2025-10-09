using System;
using System.Collections;
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

    public IEnumerator Strike(float dur = 0.5f, Action onComplete = null, bool directionIsUp = true)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 originalPos = rectTransform.localPosition;

        Vector3 lungeOffset = new Vector3(0, directionIsUp? 50f: -50f, 0); 

        // Move forward
        yield return CoroutineUtils.Lerp(dur / 2, (t) =>
        {
            rectTransform.localPosition = Vector3.Lerp(originalPos, originalPos + lungeOffset, t);
        });

        // Move back
        yield return CoroutineUtils.Lerp(dur / 2, (t) =>
        {
            rectTransform.localPosition = Vector3.Lerp(originalPos + lungeOffset, originalPos, t);
        });

        onComplete?.Invoke();
    }
}
