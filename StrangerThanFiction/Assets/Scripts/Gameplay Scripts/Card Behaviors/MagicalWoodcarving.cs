using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MagicalWoodcarving : CardModel
{
    public override string Title => "Magical Woodcarving";
    public override string Description => "At round end, I strike a random enemy, then I am destroyed.";
    public override string FlavorText => base.FlavorText;
    public override CardType Type => CardType.Unit;
    public override string PortraitPath => "CardPortraits/Magical_Woodcarving.png";

    public override int BaseCost => 1;
    public override int BasePower => 7;
    public override int BasePlotArmor => 0;

    public override void Start() => base.Start();
    
    public bool isSubscribedToRoundEnd = false;

    protected override Task SummonEffect()
    {
        Debug.Log($"MWC Summon Effect Added By: {Owner.gameObject.name}");
        if (!isSubscribedToRoundEnd)
        {
            OnRoundEnd += RoundEndEffect;
            isSubscribedToRoundEnd = true;
        }
        return Task.CompletedTask;
    }

    protected override Task DestroyEffect(CardModel card)
    {
        Debug.Log($"MWC Summon Effect Removed By: {Owner.gameObject.name}");
        if (!isSubscribedToRoundEnd)
        {
            OnRoundEnd -= RoundEndEffect;
            isSubscribedToRoundEnd = false;
        }
        return Task.CompletedTask;
    }

    protected async Task RoundEndEffect()
    {
        Debug.Log($"MWC Round End Effect Called By: {Owner.gameObject.name}");
        CardModel randomEnemy = Board.GetRandomUnit(Owner.enemyPlayer);
        if (randomEnemy)
        {
            Debug.Log($"Targeting: {randomEnemy.gameObject.name}");
            StartCoroutine(GetComponent<UnitAnim>().Strike(1.0f));
            await randomEnemy.TakeDamage(CurrentPower);
        }
        else
        {
            Debug.Log($"Targeting: Noone.");
        }

        await Destroy();
    }
}
