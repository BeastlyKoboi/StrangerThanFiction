using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCard : MonoBehaviour
{
    [SerializeField] private float _animationDuration = 5f;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private bool _hasAnimationFinished;
    public bool HasAnimationFinished { get => _hasAnimationFinished; }

    private CardLayer[] _layers;

    public void StartTransition()
    {
        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        _layers = GetComponentsInChildren<CardLayer>();

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

}
