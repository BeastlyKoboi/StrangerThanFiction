using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to define whether a card is a unit or a spell, 
/// especially when dealing with different behavior. 
/// </summary>
public enum CardType { Unit, Spell }
public enum Faction { Pinocchio, LittleRed, HumptyDumpty, TheBigBadWolf }

/// <summary>
/// Defines the basic members and behaviors for all cards. Meant to be 
/// extended into card-specific scripts rather than used as is. 
/// </summary>
public abstract class CardModel : MonoBehaviour, IDamagable, IDamageSource
{
    // ----------------------------------------------------------------------------
    // Physical Descriptors of the card, that will effect how it is viewed.
    // ----------------------------------------------------------------------------
    public static CardDataMono cardData;
    public virtual string Title { get; private set; } 
    public virtual string Description { get; private set; }
    public virtual string FlavorText { get; private set; }
    public virtual CardType Type { get; private set; }
    public virtual Sprite Portrait { get; private set; }
    public virtual bool IsHidden { get; set; } = false;

    // unused for now
    public virtual string Cardback { get; private set; }

    // ----------------------------------------------------------------------------
    // Stats that will not be changed - Consider making children implement this as static somehow?
    // ----------------------------------------------------------------------------
    public virtual int BaseCost { get; private set; }
    public virtual int BasePower { get; private set; }
    public virtual int BasePlotArmor { get; private set; }

    // ----------------------------------------------------------------------------
    // Stats that reflect gameplay and can be changed.
    //  - Max properties will automatically increase when current goes above it.
    // ----------------------------------------------------------------------------
    public virtual int MaxPower { get; set; }

    private int _currentCost;
    public virtual int CurrentCost
    {
        get { return _currentCost; }
        private set
        {
            _currentCost = value;
            cardView.UpdateCardStatText(this);
            // Add check here later for if it's a spell or unit
            cardView.UpdateUnitStatText(this);
        }
    }
    private int _currentPower;
    public virtual int CurrentPower
    {
        get { return _currentPower; }
        private set
        {
            _currentPower = value;
            cardView.UpdateCardStatText(this);
            // Add check here later for if it's a spell or unit
            cardView.UpdateUnitStatText(this);
        }
    }
    private int _currentPlotArmor;
    public virtual int CurrentPlotArmor
    {
        get { return _currentPlotArmor; }
        private set
        {
            _currentPlotArmor = value;
            cardView.UpdateCardStatText(this);
            // Add check here later for if it's a spell or unit
            cardView.UpdateUnitStatText(this);
        }
    }

    // Used in conditions like Resilient
    public virtual int DamageResistence { get; set; } = 0;

    /// <summary>
    /// Holds labeled objects for the conditions applied to a card: Resilient, Poisoned, etc.
    /// </summary>
    private Dictionary<string, Condition> conditions = new Dictionary<string, Condition>();

    /// <summary>
    /// Holds play requirements, if any: Target 1, Ally 1, etc. 
    /// </summary>
    public PlayRequirements PlayRequirements { get; private set; }

    // Used to set whether the card is playable and if it should indicate as such.
    [SerializeField] private bool _playable = true;
    public bool Playable
    {
        get { return _playable; }
        set
        {
            bool oldValue = _playable;
            //if (_playable == value) 
            //    return; 
            _playable = value;

            if (IsHidden)
            {
                GetComponent<Draggable>().enabled = false;
                return;
            }

            // Not the most efficient, but it works for now.
            cardView.ToggleGlow(value);
            GetComponent<Draggable>().enabled = value;
        }
    }

    // Quick Ref to use in stuff like conditions and internal behavior
    public Player Owner { get; set; }
    public BoardManager Board { get; set; }

    // Unit specific placement info
    public UnitRow SelectedArea { get; set; }

    // Ref to update view.
    public CardView cardView;

    // ----------------------------------------------------------------------------
    // All Card Events 
    // ----------------------------------------------------------------------------

    // Card Events - common to both units and spells.
    public UniTaskEvent<CardPlayState> OnPlay = new UniTaskEvent<CardPlayState>();
    public UniTaskEvent OnDraw = new UniTaskEvent();
    public UniTaskEvent OnDiscard = new UniTaskEvent();
    public UniTaskEvent<CardModel> OnDestroy = new UniTaskEvent<CardModel>();
    public UniTaskEvent<CardModel> OnRemove = new UniTaskEvent<CardModel>();

    // Unit Events - only called when in play, otherwise never.
    public UniTaskEvent OnDeploy = new UniTaskEvent();
    public UniTaskEvent OnSummon = new UniTaskEvent();
    public UniTaskEvent OnRoundStart = new UniTaskEvent();
    public UniTaskEvent OnRoundEnd = new UniTaskEvent();
    public UniTaskEvent<UnitStrikeState> OnStrike = new UniTaskEvent<UnitStrikeState>();
    public UniTaskEvent<DamageData> OnTakeDamage = new UniTaskEvent<DamageData>();
    public UniTaskEvent<int> OnGrantCostModification = new UniTaskEvent<int>();
    public UniTaskEvent<int> OnGrantPower = new UniTaskEvent<int>();
    public UniTaskEvent<int> OnGrantPlotArmor = new UniTaskEvent<int>();
    public UniTaskEvent OnHeal = new UniTaskEvent();
  

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        cardView = GetComponent<CardView>();
        CardInfo cardInfo = cardView.CardInfo;

        Title = cardInfo.Title;
        Description = cardInfo.Description;
        FlavorText = cardInfo.FlavorText;
        Type = cardInfo.Type;
        Portrait = cardInfo.Portrait;
        BaseCost = cardInfo.BaseCost;
        BasePower = cardInfo.BasePower;
        BasePlotArmor = cardInfo.BasePlotArmor;
        PlayRequirements = cardInfo.PlayRequirements;

        CurrentCost = BaseCost;
        CurrentPower = BasePower;
        CurrentPlotArmor = BasePlotArmor;

        MaxPower = CurrentPower;


        OnPlay.AddListener(PlayAnim);
        OnSummon.AddListener(SummonAnim);
        OnStrike.AddListener(StrikeAnim);
        OnDiscard.AddListener(DiscardAnim);
        OnDestroy.AddListener(DestroyAnim);

        OnPlay.AddListener(PlayEffect);
        OnDeploy.AddListener(DeployEffect);
        OnSummon.AddListener(SummonEffect);
        OnDiscard.AddListener(DiscardEffect);
        OnDestroy.AddListener(DestroyEffect);
        OnRemove.AddListener(RemoveEffect);

        gameObject.AddComponent<UnitAnim>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }


    // ----------------------------------------------------------------------------
    // Card Animations
    // ----------------------------------------------------------------------------
    protected virtual UniTask PlayAnim(CardPlayState cardPlayState)
    {
        return UniTask.CompletedTask;
    }
    protected virtual async UniTask SummonAnim()
    {
        float dur = 0.5f;
        StartCoroutine(gameObject.GetComponent<UnitAnim>().Summoned(dur));

        await UniTask.Delay((int)(dur * 1000));
    }
    protected virtual async UniTask StrikeAnim(UnitStrikeState unitStrikeState)
    {
        float delay = 0.5f;
        float dur = 0.25f;
        StartCoroutine(gameObject.GetComponent<UnitAnim>().Strike(1.0f));
        await UniTask.Delay((int)((delay + dur) * 1000));
    }
    protected virtual async UniTask DiscardAnim()
    {
        float delay = 0.5f;
        float dur = 0.25f;
        bool isDone = false;
        StartCoroutine(gameObject.AddComponent<Disappear>().AnimateDiscard(pulseDur: delay, discardDur: dur, () =>
        {
            isDone = true;
        }));

        while (!isDone)
        {
            await UniTask.Yield();
        }
    }
    protected virtual async UniTask DestroyAnim(CardModel card)
    {
        float delay = 0.5f;
        float dur = 0.5f;
        StartCoroutine(gameObject.AddComponent<Disappear>().AnimateDestroy(delay: delay, duration: dur));

        await UniTask.Delay((int)((delay + dur) * 1000));
    }

    protected virtual async UniTask RemoveAnim(CardModel card)
    {
        float delay = 0.5f;
        float dur = 0.5f;
        StartCoroutine(gameObject.AddComponent<Disappear>().AnimateRemove(delay: delay, duration: dur));

        await UniTask.Delay((int)((delay + dur) * 1000));
    }

    // ----------------------------------------------------------------------------
    // Card Behaviors
    // ----------------------------------------------------------------------------

    // Should be overridden by cards if they have any effects.
    protected virtual UniTask PlayEffect(CardPlayState cardPlayState)
    {
        return UniTask.CompletedTask;
    }
    protected virtual UniTask DeployEffect()
    {
        return UniTask.CompletedTask;
    }
    protected virtual UniTask SummonEffect()
    {
        return UniTask.CompletedTask;
    }
    protected virtual UniTask DiscardEffect()
    {
        return UniTask.CompletedTask;
    }
    protected virtual UniTask DestroyEffect(CardModel card)
    {
        return UniTask.CompletedTask;
    }
    protected virtual UniTask RemoveEffect(CardModel card)
    {
        return UniTask.CompletedTask;
    }


    /// <summary>
    /// Method to play this card. Needs to add Play Requirements functionality
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public async UniTask Play(CardPlayState cardPlayState)
    {
        Owner.CurrentMana -= CurrentCost;

        await OnPlay.InvokeAsync(cardPlayState);

        if (Type == CardType.Unit)
        {
            await Deploy();
            await Summon();
        }
    }

    public async UniTask<bool> Deploy()
    {
        cardView.ToggleViewType(true);

        if (SelectedArea == null)
        {
            SelectedArea = Board.GetValidRow(Owner);

            if (SelectedArea == null)
            {
                await Remove();
                return false;
            }
        }

        IsHidden = false;

        await Board.DeployUnit(this, SelectedArea);

        await OnDeploy.InvokeAsync();

        return true;
    }

    /// <summary>
    /// Method to summon this card as a unit.
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> Summon()
    {
        await Board.SummonUnit(this, SelectedArea);

        await OnSummon.InvokeAsync();

        await Owner.UnitSummoned(this);

        return true;
    }

    /// <summary>
    /// Method to discard this card.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public async UniTask Discard(Player player)
    {
        await OnDiscard.InvokeAsync();
    }

    /// <summary>
    /// Method to destroy this card. 
    /// </summary>
    /// <returns></returns>
    public async UniTask Destroy()
    {
        await OnDestroy.InvokeAsync(this);

        await Remove();
    }

    public async UniTask Remove()
    {
        OnRemove.AddListener(CardFactory.Instance.RecycleCard);
        
        // Remove all conditions 
        string[] conKeys = conditions.Keys.ToArray();
        foreach (string conditionName in conKeys)
        {
            await RemoveCondition(conditionName);
        }

        await OnRemove.InvokeAsync(this);
    }


    public async UniTask TransformInto(string newUnitName)
    {
        // Create the new unit, but don't summon yet
        CardModel newUnit = CardFactory.Instance.CreateCard(newUnitName, false, transform.parent, Owner, Board);

        newUnit.cardView.ToggleViewType(true);

        // Replace in the same board slot
        Board.ReplaceUnit(this, newUnit);

        // Fire pre-summon event to set up any necessary adjustments
        await newUnit.OnDeploy.InvokeAsync();

        // Transfer properties
        if (CurrentPower > BasePower)
            await newUnit.GrantPower(CurrentPower - BasePower);

        if (CurrentPlotArmor > BasePlotArmor)
            await newUnit.GrantPlotArmor(CurrentPlotArmor - BasePlotArmor);

        // Transfer conditions
        foreach (var condition in this.GetConditions())
        {
            await newUnit.ApplyCondition(condition);
        }

        // Remove old unit safely
        await this.Remove();
    }


    /// <summary>
    /// Method to trigger OnRoundStart event.
    /// </summary>
    public async UniTask RoundStart()
    {
        await OnRoundStart.InvokeAsync();
    }

    public async UniTask RoundEnd()
    {
        await OnRoundEnd.InvokeAsync();
    }

    public async UniTask Strike(IDamagable target)
    {
        await target.TakeDamage(new DamageData(damage: CurrentPower, source: this));

        await OnStrike.InvokeAsync(new UnitStrikeState(this, target));
    }


    /// <summary>
    /// Method to damage the unit and return the actual amount of damage given. 
    ///  - This excludes overkill damage
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="ignorePlotArmor">Whether the damage is affected by plot armor.</param>
    /// <returns></returns>
    public async UniTask TakeDamage(DamageData damageData)
    {
        // Should NOT be called if in card form.
        if (Type != CardType.Unit) return;

        if (CurrentPower == 0) return;

        // Applies damage mitigation effects, and separate conditions.
        damageData.damage -= DamageResistence;

        // TODO: Make ifs for helpless or invincible

        // Now that final damage is calculated, 
        // Plot armor and then power are affected in that order
        if (!damageData.ignorePlotArmor)
        {
            if (CurrentPlotArmor <= damageData.damage)
            {
                damageData.damage -= CurrentPlotArmor;
                CurrentPlotArmor = 0;
            }
            else
            {
                CurrentPlotArmor -= damageData.damage;
                damageData.damage = 0;
            }
        }

        // Damage does its worst, the OnTakeDamage event triggers,
        // and the amount is finally returned
        CurrentPower = Math.Max(CurrentPower - damageData.damage, 0);

        await OnTakeDamage.InvokeAsync(damageData);

        Owner.uiManager.UpdateTotalPower();

        if (CurrentPower == 0)
            await this.Destroy();
    }

    /// <summary>
    /// Method to increase or decrease the cost of a card. 
    /// </summary>
    /// <param name="costMod"></param>
    /// <returns></returns>
    public async UniTask GrantCostModification(int costMod)
    {
        CurrentCost = Math.Max(0, CurrentCost + costMod);

        await OnGrantCostModification.InvokeAsync(costMod);
    }

    /// <summary>
    /// Method to grant Power to this card and return the amount granted. 
    /// </summary>
    /// <param name="powerAmount"></param>
    /// <returns></returns>
    public async UniTask GrantPower(int powerAmount)
    {
        MaxPower += powerAmount;
        CurrentPower += powerAmount;

        await OnGrantPower.InvokeAsync(powerAmount);

        Owner.uiManager.UpdateTotalPower();
    }

    /// <summary>
    /// Method to grant Plot Armor to this card and return the amount granted.
    /// </summary>
    /// <param name="armorAmount"></param>
    /// <returns></returns>
    public async UniTask GrantPlotArmor(int armorAmount)
    {
        CurrentPlotArmor += armorAmount;

        await OnGrantPlotArmor.InvokeAsync(armorAmount);
    }

    /// <summary>
    /// Method to heal this unit by aan amount up to the max power.
    /// </summary>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public async UniTask Heal(int healAmount)
    {
        // Should NOT be called if in card form.
        if (Type != CardType.Unit) return;

        // should never be negative
        int totalHealPossible = MaxPower - CurrentPower;

        if (totalHealPossible == 0) return;

        healAmount = Math.Min(totalHealPossible, healAmount);

        CurrentPower += healAmount;

        await OnHeal.InvokeAsync();

        Owner.uiManager.UpdateTotalPower();
    }

    // ----------------------------------------------------------------------------
    // Unit and Card Conditions
    // ----------------------------------------------------------------------------

    /// <summary>
    /// Method to add a condition. If the condition is already added, 
    /// calls surplus implementation instead. 
    /// </summary>
    /// <param name="conditionName"></param>
    /// <param name="condition"></param>
    public async UniTask ApplyCondition(Condition condition)
    {
        if (!conditions.ContainsKey(condition.Name))
        {
            conditions.Add(condition.Name, condition);
            await condition.OnAdd();
        }
        else
        {
            await conditions[condition.Name].OnSurplus(condition);
        }
    }

    /// <summary>
    /// Method to remove a condition if possible
    /// </summary>
    /// <param name="conditionName"></param>
    public async UniTask RemoveCondition(string conditionName)
    {
        if (conditions.ContainsKey(conditionName))
        {
            await conditions[conditionName].OnRemove();
            conditions.Remove(conditionName);
        }
    }

    /// <summary>
    /// Method to trigger a condition, if possible
    /// </summary>
    /// <param name="conditionName"></param>
    public async UniTask TriggerCondition(string conditionName)
    {
        if (conditions.ContainsKey(conditionName))
        {
            await conditions[conditionName].OnTrigger();
        }
    }

    /// <summary>
    /// Method to find out if this unit has a condition
    /// </summary>
    /// <param name="conditionName"></param>
    /// <returns></returns>
    public bool HasCondition(string conditionName)
    {
        return conditions.ContainsKey(conditionName);
    }

    public Condition[] GetConditions()
    {
        return conditions.Values.ToArray();
    }

}
