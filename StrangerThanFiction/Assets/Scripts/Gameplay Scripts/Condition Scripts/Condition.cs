using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Condition : IDamageSource
{
    public static ConditionsDataMono conditionsData = GameObject.Find("ConditionsData").GetComponent<ConditionsDataMono>();
    public virtual string Name { get; } = "";
    public virtual string Description { get; } = "";
    public virtual ConditionType ConditionType { get; set; }
    public virtual Sprite Icon { get; set; }

    protected CardModel card;
    public int amount;

    public Condition(CardModel card, int amount)
    {
        ConditionInfo conditionInfo = conditionsData.conditionsDictionary.GetByKey(GetType().ToString());

        this.Name = conditionInfo.ConditionName;
        this.Description = conditionInfo.Description;
        this.ConditionType = conditionInfo.ConditionType;
        this.Icon = conditionInfo.Image;
        this.card = card;
        this.amount = amount;
    }

    public virtual UniTask OnAdd() => UniTask.CompletedTask;
    public virtual UniTask OnTrigger() => UniTask.CompletedTask;
    public virtual UniTask OnSurplus(Condition surplus) => UniTask.CompletedTask;
    public virtual UniTask OnRemove() => UniTask.CompletedTask;

    public override string ToString() => $"<line-indent=30>{Name} {amount}: {Description}";
}
