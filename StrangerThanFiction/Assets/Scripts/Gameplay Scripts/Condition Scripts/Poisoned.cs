using Cysharp.Threading.Tasks;

public class Poisoned : Condition
{
    public override uint Id => 0;

    public Poisoned(CardModel card, int amount) : base(card, amount) { }

    public override UniTask OnAdd()
    {
        card.Owner.OnRoundEnd.AddListener(OnTrigger);
        return UniTask.CompletedTask;
    }
    public override async UniTask OnTrigger()
    {
        await card.TakeDamage(amount, true);
        amount -= 1;
        if (amount == 0)
        {
            await card.RemoveCondition(Name);
        }
    }
    public override UniTask OnSurplus(Condition surplus)
    {
        if (surplus is Poisoned poisonSurplus)
        {
            amount += poisonSurplus.amount;
        }
        return UniTask.CompletedTask;
    }
    public override UniTask OnRemove()
    {
        card.Owner.OnRoundEnd.RemoveListener(OnTrigger);
        return UniTask.CompletedTask;
    }
}
