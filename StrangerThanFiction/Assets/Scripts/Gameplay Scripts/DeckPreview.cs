using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPreview : MonoBehaviour
{
    public Transform tooltip;
    public Transform backPanel;
    public CanvasGroup tooltipCG;
    public bool isFading;
    public float fadeDuration = 0.20f;

    public void Unfocus()
    {
        if (isFading) return;
        isFading = true;
        backPanel.gameObject.SetActive(false);
        StartCoroutine(Fade(dur: fadeDuration, start: 1.0f, end: 0.0f, onComplete: () => isFading = false));
    }

    public void Focus()
    {
        if (isFading) return;
        isFading = true;
        backPanel.gameObject.SetActive(true);
        StartCoroutine(Fade(dur: fadeDuration, start: 0.0f, end: 1.0f, onComplete: () => isFading = false));
    }

    public IEnumerator Fade(float dur = 0.5f, float start = 0.0f, float end = 1.0f, Action onComplete = null)
    {
        yield return CoroutineUtils.Lerp(dur, (t) =>
        {
            tooltipCG.alpha = Mathf.Lerp(start, end, t);
        });

        onComplete?.Invoke();
    }
}
