using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    UniTask TakeDamage(DamageData damageData);
}
