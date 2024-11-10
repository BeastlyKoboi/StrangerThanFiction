using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DoubleLife : CardModel
{
    /// <summary>
    /// Needs to wait for responsive description implemntation before usning this card makes sense.
    /// </summary>
    private int _damage = 8;

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyCardTargets[0].TakeDamage(_damage);

        _damage /= 2;
    }
}
