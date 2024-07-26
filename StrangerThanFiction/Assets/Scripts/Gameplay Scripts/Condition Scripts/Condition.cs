using System.Threading.Tasks;

public class Condition 
{
    // public virtual string GetName() => typeof(Condition).ToString();
    public static string StaticName => typeof(Condition).Name;
    public virtual string Name => StaticName;
    public virtual string Description { get; } = "";

    protected CardModel card;
    public int amount;

    public Condition(CardModel card, int amount)
    {
        this.card = card;
        this.amount = amount;
    }

    public virtual Task OnAdd() => Task.CompletedTask;
    public virtual Task OnTrigger() => Task.CompletedTask;
    public virtual Task OnSurplus(Condition surplus) => Task.CompletedTask;
    public virtual Task OnRemove() => Task.CompletedTask;
}
