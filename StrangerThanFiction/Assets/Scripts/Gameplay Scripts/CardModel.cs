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
public abstract class CardModel : MonoBehaviour
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
            UpdateCardStatText();

            if (unitView)
                UpdateUnitStatText();
        }
    }
    private int _currentPower;
    public virtual int CurrentPower
    {
        get { return _currentPower; }
        private set
        {
            _currentPower = value;
            UpdateCardStatText();

            if (Type == CardType.Unit)
                UpdateUnitStatText();
        }
    }
    private int _currentPlotArmor;
    public virtual int CurrentPlotArmor
    {
        get { return _currentPlotArmor; }
        private set
        {
            _currentPlotArmor = value;
            UpdateCardStatText();

            if (Type == CardType.Unit)
                UpdateUnitStatText();
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
            cardView.Find("Glow").gameObject.SetActive(value);
            GetComponent<Draggable>().enabled = value;
        }
    }

    // Quick Ref to use in stuff like conditions and internal behavior
    public Player Owner { get; set; }
    public BoardManager Board { get; set; }

    // Unit specific placement info
    public UnitRow SelectedArea { get; set; }

    // Ref to update view.
    public Transform cardView;
    private TextMeshProUGUI cardTextCost;
    private TextMeshProUGUI cardTextPower;
    private TextMeshProUGUI cardTextPlotArmor;

    public Transform unitView;
    private TextMeshProUGUI unitTextPower;
    private TextMeshProUGUI unitTextPlotArmor;


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
    public UniTaskEvent OnSummon = new UniTaskEvent();
    public UniTaskEvent OnRoundStart = new UniTaskEvent();
    public UniTaskEvent OnRoundEnd = new UniTaskEvent();
    public UniTaskEvent<UnitStrikeState> OnStrike = new UniTaskEvent<UnitStrikeState>();
    public UniTaskEvent<int> OnTakeDamage = new UniTaskEvent<int>();
    public UniTaskEvent<int> OnGrantCostModification = new UniTaskEvent<int>();
    public UniTaskEvent<int> OnGrantPower = new UniTaskEvent<int>();
    public UniTaskEvent<int> OnGrantPlotArmor = new UniTaskEvent<int>();
    public UniTaskEvent OnHeal = new UniTaskEvent();
  

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        if (cardData == null)
            cardData = GameObject.Find("CardData").GetComponent<CardDataMono>();

        CardInfo cardInfo = cardData.cardDictionary.GetCardDataByName(name);

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
        OnDiscard.AddListener(DiscardAnim);
        OnDestroy.AddListener(DestroyAnim);

        OnPlay.AddListener(PlayEffect);
        OnSummon.AddListener(SummonEffect);
        OnDiscard.AddListener(DiscardEffect);
        OnDestroy.AddListener(DestroyEffect);
        OnRemove.AddListener(RemoveEffect);
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
        StartCoroutine(gameObject.AddComponent<UnitAnim>().Summoned(dur));

        await UniTask.Delay((int)(dur * 1000));
    }
    protected virtual async UniTask DiscardAnim()
    {
        float delay = 0.5f;
        float dur = 0.25f;
        StartCoroutine(gameObject.AddComponent<Disappear>().AnimateDiscard(pulseDur: delay, discardDur: dur));

        await UniTask.Delay((int)((delay + dur) * 1000));
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
            await Summon();
        }
    }

    /// <summary>
    /// Method to summon this card as a unit.
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> Summon()
    {
        cardView.gameObject.SetActive(false);
        unitView.gameObject.SetActive(true);

        if (SelectedArea == null)
        {
            SelectedArea = Board.GetRandomValidRow(Owner);

            if (SelectedArea == null)
            {
                await Remove();
                return false;
            }
        }

        await Board.SummonUnit(this, SelectedArea);

        await OnSummon.InvokeAsync();

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

    public async UniTask Strike(CardModel target)
    {
        await target.TakeDamage(CurrentPower);

        await OnStrike.InvokeAsync(new UnitStrikeState(this, target));
    }

    /// <summary>
    /// Method to damage the unit and return the actual amount of damage given. 
    ///  - This excludes overkill damage
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="ignorePlotArmor">Whether the damage is affected by plot armor.</param>
    /// <returns></returns>
    public async UniTask TakeDamage(int damage, bool ignorePlotArmor = false)
    {
        // Should NOT be called if in card form.
        if (Type != CardType.Unit) return;

        if (CurrentPower == 0) return;

        // Applies damage mitigation effects, and separate conditions.
        damage -= DamageResistence;

        // TODO: Make ifs for helpless or invincible

        // Now that final damage is calculated, 
        // Plot armor and then power are affected in that order
        if (!ignorePlotArmor)
        {
            if (CurrentPlotArmor <= damage)
            {
                damage -= CurrentPlotArmor;
                CurrentPlotArmor = 0;
            }
            else
            {
                CurrentPlotArmor -= damage;
                damage = 0;
            }
        }

        // Damage does its worst, the OnTakeDamage event triggers,
        // and the amount is finally returned
        CurrentPower = Math.Max(CurrentPower - damage, 0);

        await OnTakeDamage.InvokeAsync(damage);

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

    // ----------------------------------------------------------------------------
    // Loading Assets & Overwriting Card Prefabs
    // ----------------------------------------------------------------------------

    /// <summary>
    /// At instantiation will be used to overwrite placeholder card gameobject
    /// with correct sprites and initial values.
    /// </summary>
    public void OverwriteCardPrefab()
    {
        cardView = transform.Find("CardPrefab(Clone)");
        cardView.gameObject.SetActive(true);

        if (cardView == null)
            return;

        // Load portrait picture
        Transform portrait = cardView.Find("Portrait");
        portrait.GetComponent<Image>().sprite = Portrait;

        cardTextCost = cardView.Find("Cost").GetComponent<TextMeshProUGUI>();
        cardTextCost.text = CurrentCost.ToString();


        Transform spellBackground = cardView.Find("SpellBackground");
        Transform unitBackground = cardView.Find("UnitBackground");

        if (Type == CardType.Spell)
        {
            spellBackground.gameObject.SetActive(true);
            unitBackground.gameObject.SetActive(false);

            cardView.Find("Power").gameObject.SetActive(false);
            cardView.Find("PlotArmor").gameObject.SetActive(false);
        }
        else
        {
            unitBackground.gameObject.SetActive(true);
            spellBackground.gameObject.SetActive(false);

            cardTextPower = cardView.Find("Power").GetComponent<TextMeshProUGUI>();
            cardTextPower.text = CurrentPower.ToString();
            cardTextPlotArmor = cardView.Find("PlotArmor").GetComponent<TextMeshProUGUI>();
            cardTextPlotArmor.text = CurrentPlotArmor.ToString();
        }

        cardView.Find("Name").GetComponent<TextMeshProUGUI>().text = Title;
        cardView.Find("Description").GetComponent<TextMeshProUGUI>().text = Description;
        cardView.Find("Cardback").gameObject.SetActive(IsHidden);
    }

    /// <summary>
    /// At instantiation will be used to overwrite placeholder unit gameobject
    /// with correct sprites and initial values. 
    /// </summary>
    public void OverwriteUnitPrefab()
    {
        unitView = transform.Find("UnitPrefab(Clone)");

        if (unitView == null)
            return;

        // Load portrait picture
        Sprite sprite = Portrait;
        Transform portrait = unitView.Find("Portrait");

        if (sprite != null)
            portrait.GetComponent<Image>().sprite = sprite;

        unitTextPower = unitView.Find("Power").GetComponent<TextMeshProUGUI>();
        unitTextPower.text = CurrentPower.ToString();

        unitTextPlotArmor = unitView.Find("PlotArmor").GetComponent<TextMeshProUGUI>();
        unitTextPlotArmor.text = CurrentPlotArmor.ToString();

        unitView.Find("Name").GetComponent<TextMeshProUGUI>().text = Title;

        unitView.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the card stat text on the card view.
    /// </summary>
    private void UpdateCardStatText()
    {
        if (!cardView) return;
        cardTextCost.text = CurrentCost.ToString();

        // Spells need to be updated, but will not have these saved. 
        if (cardTextPower)
        {
            cardTextPower.text = CurrentPower.ToString();
            if (CurrentPower > BasePower)
                cardTextPower.color = Color.green;
            else
                cardTextPower.color = Color.white;
        }
        if (cardTextPlotArmor)
        {
            cardTextPlotArmor.text = CurrentPlotArmor.ToString();
            if (CurrentPlotArmor > BasePlotArmor)
                cardTextPlotArmor.color = Color.green;
            else
                cardTextPlotArmor.color = Color.white;
        }
    }

    /// <summary>
    /// Updates the unit stat text on the unit view.
    /// </summary>
    private void UpdateUnitStatText()
    {
        if (!unitView) return;
        unitTextPower.text = CurrentPower.ToString();
        unitTextPlotArmor.text = CurrentPlotArmor.ToString();

        if (CurrentPower < MaxPower)
            unitTextPower.color = Color.red;
        else if (CurrentPower > BasePower)
            unitTextPower.color = Color.green;
        else
            unitTextPower.color = Color.white;

        if (CurrentPlotArmor > BasePlotArmor)
            unitTextPlotArmor.color = Color.green;
        else if (CurrentPlotArmor < BasePlotArmor)
            unitTextPlotArmor.color = Color.yellow;
        else
            unitTextPlotArmor.color = Color.white;
    }
}
