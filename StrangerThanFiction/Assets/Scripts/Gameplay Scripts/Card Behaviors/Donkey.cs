using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Donkey : CardModel
{
    protected async override void Awake()
    {
        base.Awake();

        await ApplyCondition(new Stackable(this, 0));
    }
}
