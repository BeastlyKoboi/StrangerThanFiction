using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class Boon : ISelectable
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

    public virtual UniTask FirstAdded() => UniTask.CompletedTask;
    public virtual UniTask EnterCombat(Player player) => UniTask.CompletedTask;
    public virtual UniTask ExitCombat() => UniTask.CompletedTask;
    public virtual UniTask EnterNodemap() => UniTask.CompletedTask;
    public virtual UniTask ExitNodemap() => UniTask.CompletedTask;
    public virtual UniTask EnterEncounter() => UniTask.CompletedTask;
    public virtual UniTask ExitEncounter() => UniTask.CompletedTask;
    public virtual UniTask Remove() => UniTask.CompletedTask;

    public override string ToString() => $"{Name}: {Description}";
}
