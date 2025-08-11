using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class PlayedCardView : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private GameObject enemyCardView;

    private CardPlayState cardPlayState;

    // Start is called before the first frame update
    void Start()
    {
        player.OnBeforeCardPlayed.AddListener(CardPreview);
    }

    private async UniTask CardPreview(CardPlayState cardPlayState)
    {
        this.cardPlayState = cardPlayState;
        cardPlayState.card.transform.SetParent(enemyCardView.transform, true);
        cardPlayState.card.IsHidden = false;

        StartCoroutine(MoveToView(1f, null));

        await UniTask.Delay(4000);
    }

    public IEnumerator MoveToView(float dur = 0.5f, Action onComplete = null)
    {
        Vector3 startPos = cardPlayState.card.transform.localPosition;
        Quaternion startRot = cardPlayState.card.transform.localRotation;
        Vector3 startScale = cardPlayState.card.transform.localScale;

        yield return CoroutineUtils.Lerp(dur, (t) =>
        {
            cardPlayState.card.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero, t);
            cardPlayState.card.transform.localRotation = Quaternion.Lerp(startRot, Quaternion.Euler(0,0,0), t);
            cardPlayState.card.transform.localScale = Vector3.Lerp(startScale, Vector3.one * 1.5f, t);
        });

        onComplete?.Invoke();
    }

}
