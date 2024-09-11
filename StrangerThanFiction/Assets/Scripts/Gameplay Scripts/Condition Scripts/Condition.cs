using System.Threading.Tasks;
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

    public virtual Task OnAdd() => Task.CompletedTask;
    public virtual Task OnTrigger() => Task.CompletedTask;
    public virtual Task OnSurplus(Condition surplus) => Task.CompletedTask;
    public virtual Task OnRemove() => Task.CompletedTask;

    public override string ToString() => $"{Name} {amount}: {Description}";
}
