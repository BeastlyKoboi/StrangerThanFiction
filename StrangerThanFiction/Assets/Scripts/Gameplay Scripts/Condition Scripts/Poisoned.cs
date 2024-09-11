using System.Threading.Tasks;

public class Poisoned : Condition
{
    public override uint Id => 0;

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
