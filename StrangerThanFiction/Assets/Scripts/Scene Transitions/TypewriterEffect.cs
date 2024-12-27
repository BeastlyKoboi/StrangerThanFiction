using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    TextMeshProUGUI text;
    TMP_TextInfo textInfo;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        textInfo = text.textInfo;
        text.ForceMeshUpdate();
        text.maxVisibleCharacters = 0;
    }

    public void StartTypeWriter(float speed)
    {
        StartCoroutine(AnimateText(speed));
    }

    private IEnumerator AnimateText(float speed = 20)
    {
        text.ForceMeshUpdate();

        int totalChars = textInfo.characterCount;
        int visibleChars = 0;
        float delay = 1f / speed;
        while (visibleChars != totalChars)
        {
            visibleChars += 1;
            text.maxVisibleCharacters = visibleChars;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
