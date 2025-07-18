using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class DoubleLife : CardModel
{
    /// <summary>
    /// Needs to wait for responsive description implemntation before usning this card makes sense.
    /// </summary>
    private int _damage = 8;

    protected override async UniTask PlayEffect(CardPlayState cardPlayState)
    {
        await cardPlayState.enemyCardTargets[0].TakeDamage(new DamageData(_damage, this));

        _damage /= 2;
    }
}
