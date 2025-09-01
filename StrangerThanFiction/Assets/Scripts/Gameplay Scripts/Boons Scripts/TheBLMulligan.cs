using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBLMulligan : Boon
{
    public override UniTask EnterEncounter(SpecialNode specialNode)
    {
        specialNode.IncrementFreeRerolls();
        return UniTask.CompletedTask;
    }
}
