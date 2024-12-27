using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransitionCard : MonoBehaviour
{
    [SerializeField] private float _animationDuration = 5f;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private bool _hasAnimationFinished;
    public bool HasAnimationFinished { get => _hasAnimationFinished; }

    private CardLayer[] _layers;
    private TypewriterEffect _typewriter;
    private TextMeshProUGUI[] _textArr;

    private void Start()
    {
        _textArr = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in _textArr)
        {
            Color color = text.color;
            color.a = 0;
            text.color = color;
        }

    }

    public void StartTransition()
    {
        StartCoroutine(Transition());
        StartCoroutine(TextFadeIn(_animationDuration));
    }

    IEnumerator Transition()
    {
        _layers = GetComponentsInChildren<CardLayer>();
        //_typewriter = GetComponentInChildren<TypewriterEffect>();

        //if (_typewriter != null)
        //{
        //    _typewriter.StartTypeWriter(20f);
        //}

        float timer = 0;

        while (timer < _animationDuration)
        {
            float t = _animationCurve.Evaluate(timer / _animationDuration);

            foreach (CardLayer layer in _layers)
            {
                layer.Move(t);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        foreach (CardLayer layer in _layers)
        {
            layer.Move(1);
        }

        _hasAnimationFinished = true;
    }

    private IEnumerator TextFadeIn(float duration)
    {
        if (_textArr.Length == 0)
        {
            yield break;
        }

        float timer = 0;

        for (int i = 0; i < _textArr.Length; i++)
        {
            _textArr[i].color = new Color(_textArr[i].color.r, _textArr[i].color.g, _textArr[i].color.b, 0);

            while (timer < duration / (_textArr.Length + 1))
            {
                float t = timer / (duration / (_textArr.Length + 1));
                _textArr[i].color = new Color(_textArr[i].color.r, _textArr[i].color.g, _textArr[i].color.b, t);
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;

            yield return null;
        }
    }

}
