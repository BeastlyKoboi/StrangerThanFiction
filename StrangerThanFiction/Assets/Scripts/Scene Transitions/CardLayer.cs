using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayer : MonoBehaviour
{
    [SerializeField] private Vector3 _startPosition;
    [SerializeField] private Vector3 _targetPosition;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Move(float t)
    {
        _rectTransform.anchoredPosition3D = Vector3.Lerp(_startPosition, _targetPosition, t);
    }
}
