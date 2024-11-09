using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePulse : MonoBehaviour
{
    private bool isPulsing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isPulsing)
        {
            isPulsing = true;
            StartCoroutine(Pulse(onComplete: () => isPulsing = false));
        }

    }



    public IEnumerator Pulse(float dur = 1.0f, Action onComplete = null)
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
}
