using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Condition 
{
    public static ConditionsDataMono conditionsData = GameObject.Find("ConditionsData").GetComponent<ConditionsDataMono>();
    public abstract uint Id { get; }
    public virtual string Name { get; } = "";
    public virtual string Description { get; } = "";

    protected CardModel card;
    public int amount;

    public Condition(CardModel card, int amount)
    {
        this.Name = conditionsData.conditionsList.conditions[Id].ConditionName;
        this.Description = conditionsData.conditionsList.conditions[Id].Description;
        this.card = card;
        this.amount = amount;
    }

    public virtual UniTask OnAdd() => UniTask.CompletedTask;
    public virtual UniTask OnTrigger() => UniTask.CompletedTask;
    public virtual UniTask OnSurplus(Condition surplus) => UniTask.CompletedTask;
    public virtual UniTask OnRemove() => UniTask.CompletedTask;

    public override string ToString() => $"{Name} {amount}: {Description}";
}
