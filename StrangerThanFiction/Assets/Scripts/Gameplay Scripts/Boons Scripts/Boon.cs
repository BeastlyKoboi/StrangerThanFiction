using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class Boon 
{
    public static BoonsDataMono boonsData = GameObject.Find("BoonsData").GetComponent<BoonsDataMono>();
    public virtual string Name { get; } = "";
    public virtual string Description { get; } = "";

    public Boon()
    {
        BoonInfo boonInfo = boonsData.boonDictionary.GetByKey(GetType().ToString());

        this.Name = boonInfo.name;
        this.Description = boonInfo.Description;
    }

    public virtual UniTask OnAdd() => UniTask.CompletedTask;
    public virtual UniTask OnEnterCombat() => UniTask.CompletedTask;
    public virtual UniTask OnExitCombat() => UniTask.CompletedTask;
    public virtual UniTask OnRemove() => UniTask.CompletedTask;



    public override string ToString() => $"{Name}: {Description}";
}
