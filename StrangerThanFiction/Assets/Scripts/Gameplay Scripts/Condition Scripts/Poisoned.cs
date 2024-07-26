using System.Threading.Tasks;

public class Poisoned : Condition
{
    public static new string StaticName => typeof(Poisoned).Name;
    public override string Name => StaticName;
    public override string Description { get; } = "On Round End, unit takes # damage. Does stack.";

    public Poisoned(CardModel card, int amount) : base(card, amount) { }

    public override Task OnAdd()
    {
        card.Owner.OnRoundEnd += OnTrigger;
        return Task.CompletedTask;
    }
    public override async Task OnTrigger()
    {
        await card.TakeDamage(amount, true);
        amount -= 1;
        if (amount == 0)
        {
            await card.RemoveCondition(Name);
        }
    }
    public override Task OnSurplus(Condition surplus)
    {
        if (surplus is Poisoned poisonSurplus)
        {
            amount += poisonSurplus.amount;
        }
        return Task.CompletedTask;
    }
    public override Task OnRemove()
    {
        card.Owner.OnRoundEnd -= OnTrigger;
        return Task.CompletedTask;
    }
}
