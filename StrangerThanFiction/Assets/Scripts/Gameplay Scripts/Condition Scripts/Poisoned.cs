using System.Threading.Tasks;

public class Poisoned : ICondition
{
    private readonly CardModel card;
    public int amount;

    public Poisoned(CardModel card, int amount)
    {
        this.card = card;
        this.amount = amount;
    }

    public static string GetName()
    {
        return "Poisoned";
    }

    public Task OnAdd()
    {
        card.OnRoundEnd += OnTrigger;
        return Task.CompletedTask;
    }
    public async Task OnTrigger()
    {
        await card.TakeDamage(amount, true);
        amount -= 1;
    }
    public Task OnSurplus(ICondition surplus)
    {
        if (surplus is Poisoned poisonSurplus)
        {
            amount += poisonSurplus.amount;
        }
        return Task.CompletedTask;
    }
    public Task OnRemove()
    {
        card.OnRoundEnd -= OnTrigger;
        return Task.CompletedTask;
    }
}
