using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to define whether a card is a unit or a spell, 
/// especially when dealing with different behavior. 
/// </summary>
public enum CardType { Unit, Spell }

/// <summary>
/// Defines the basic members and behaviors for all cards. Meant to be 
/// extended into card-specific scripts rather than used as is. 
/// </summary>
public abstract class CardModel : MonoBehaviour
{
    // ----------------------------------------------------------------------------
    // Physical Descriptors of the card, that will effect how it is viewed.
    // ----------------------------------------------------------------------------
    public virtual string Title { get; }
    public virtual string Description { get; }
    public virtual string FlavorText { get; }
    public virtual CardType Type { get; set; }

    // These 3 paths are unused now, but intended for the ability to have card skins.
    public virtual string CardbackPath { get; } = "Cardback_Placeholder.png";
    public virtual string UnitFramePath { get; } = "UnitCardFrontFrame.png"; //
    public virtual string SpellFramePath { get; } = "SpellCardFrontFrame.png"; //
    public virtual string PortraitPath { get; set; }
    public virtual bool IsHidden { get; set; } = false;

    // ----------------------------------------------------------------------------
    // Future place to add reqs for targeting ally/enemy units, and ally cards.
    // ----------------------------------------------------------------------------


    // ----------------------------------------------------------------------------
    // Stats that will not be changed - Consider making children implement this as static somehow?
    // ----------------------------------------------------------------------------
    public virtual int BaseCost { get; }
    public virtual int BasePower { get; } = 0;
    public virtual int BasePlotArmor { get; } = 0;

    // ----------------------------------------------------------------------------
    // Stats that reflect gameplay and can be changed.
    //  - Max properties will automatically increase when current goes above it.
    // ----------------------------------------------------------------------------
    public virtual int MaxCost { get; set; }
    public virtual int MaxPower { get; set; }
    public virtual int MaxPlotArmor { get; set; }


    private int _currentCost;
    public virtual int CurrentCost
    {
        get { return _currentCost; }
        set
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
        set
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
        set
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
    private Dictionary<string, ICondition> conditions = new Dictionary<string, ICondition>();

    /// <summary>
    /// Holds play requirements, if any: Target 1, Ally 1, etc. 
    /// </summary>
    public Dictionary<string, int> PlayRequirements { get; set; }

    // Used to set whether the card is playable and if it should indicate as such.
    [SerializeField] private bool _playable = true;
    public bool Playable
    {
        get { return _playable; }
        set
        {
            //if (_playable == value) 
            //    return; 
            _playable = value;

            if (IsHidden && _playable)
                return;

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
    public event Func<Task> OnPlay;
    public event Func<Task> OnDraw;
    public event Func<Task> OnDiscard;
    public event Func<CardModel, Task> OnDestroy;

    // Unit Events - only called when in play, otherwise never.
    public event Func<Task> OnSummon;
    public event Func<Task> OnRoundStart;
    public event Func<Task> OnRoundEnd;
    public event Func<int, Task> OnTakeDamage;
    public event Func<int, Task> OnGrantPower;
    public event Func<int, Task> OnGrantPlotArmor;
    public event Func<int, Task> OnHeal;


    private void Awake()
    {
        CurrentCost = BaseCost;
        CurrentPower = BasePower;
        CurrentPlotArmor = BasePlotArmor;

        MaxCost = CurrentCost;
        MaxPower = CurrentPower;
        MaxPlotArmor = CurrentPlotArmor;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        //OverwriteCardPrefab();

        //if (Type == CardType.Unit)
        //{
        //    OverwriteUnitPrefab();
        //}
        OnPlay += PlayAnim;
        OnSummon += SummonAnim;
        OnDiscard += DiscardAnim;
        OnDestroy += DestroyAnim;

        OnPlay += PlayEffect;
        OnSummon += SummonEffect;
        OnDiscard += DiscardEffect;
        OnDestroy += DestroyEffect;
    }


    // ----------------------------------------------------------------------------
    // Card Animations
    // ----------------------------------------------------------------------------
    protected virtual Task PlayAnim()
    {
        return Task.CompletedTask;
    }
    protected virtual async Task SummonAnim()
    {
        float dur = 0.5f;
        StartCoroutine(gameObject.AddComponent<UnitAnim>().Summoned(dur));

        await Task.Delay((int)(dur * 1000));
    }
    protected virtual async Task DiscardAnim()
    {
        float delay = 0.5f;
        float dur = 0.25f;
        StartCoroutine(gameObject.AddComponent<Disappear>().AnimateDiscard(pulseDur: delay, discardDur: dur));

        await Task.Delay((int)((delay + dur) * 1000));
    }
    protected virtual async Task DestroyAnim(CardModel card)
    {
        Debug.Log("Destroy Anim Called");
        float delay = 0.5f;
        float dur = 0.5f;
        StartCoroutine(gameObject.AddComponent<Disappear>().AnimateDestroy(delay: delay, duration: dur, () => Debug.Log("Destroy Anim Finished")));

        await Task.Delay((int)((delay + dur) * 1000));
        Debug.Log("Destroy Anim Task Delay Finished");

    }

    // ----------------------------------------------------------------------------
    // Card Behaviors
    // ----------------------------------------------------------------------------

    // Should be overridden by cards if they have any effects.
    protected virtual Task PlayEffect()
    {
        return Task.CompletedTask;
    }
    protected virtual Task SummonEffect()
    {
        return Task.CompletedTask;
    }
    protected virtual Task DiscardEffect()
    {
        return Task.CompletedTask;
    }
    protected virtual Task DestroyEffect(CardModel card)
    {
        return Task.CompletedTask;
    }


    /// <summary>
    /// Method to play this card. Needs to add Play Requirements functionality
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public async Task Play(CardPlayState cardPlayState)
    {
        Owner.CurrentMana -= CurrentCost;

        if (OnPlay != null)
        {
            foreach (Func<Task> handler in OnPlay.GetInvocationList()
                    .Cast<Func<Task>>())
            {
                await handler();
            }
        }

        if (Type == CardType.Unit)
        {
            await Summon();
        }
    }

    /// <summary>
    /// Method to summon this card as a unit.
    /// </summary>
    /// <returns></returns>
    public async Task Summon()
    {
        cardView.gameObject.SetActive(false);
        unitView.gameObject.SetActive(true);
        Board.SummonUnit(this, SelectedArea);
        if (OnSummon != null)
        {
            foreach (Func<Task> handler in OnSummon.GetInvocationList()
                    .Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    /// <summary>
    /// Method to discard this card.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public async Task Discard(Player player)
    {
        if (OnDiscard != null)
        {
            foreach (Func<Task> handler in OnDiscard.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    /// <summary>
    /// Method to destroy this card. 
    /// </summary>
    /// <returns></returns>
    public async Task Destroy()
    {
        OnDestroy += CardFactory.Instance.RecycleCard;

        if (OnDestroy != null)
        {
            foreach (Func<CardModel, Task> handler in OnDestroy.GetInvocationList()
                .Cast<Func<CardModel, Task>>())
            {
                await handler(this);
            }
        }
    }

    /// <summary>
    /// Method to trigger OnRoundStart event.
    /// </summary>
    public async Task RoundStart()
    {
        if (OnRoundStart != null)
        {
            foreach (Func<Task> handler in OnRoundStart.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    public async Task RoundEnd()
    {
        if (OnRoundEnd != null)
        {
            foreach (Func<Task> handler in OnRoundEnd.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    /// <summary>
    /// Method to damage the unit and return the actual amount of damage given. 
    ///  - This excludes overkill damage
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="ignorePlotArmor">Whether the damage is affected by plot armor.</param>
    /// <returns></returns>
    public async Task TakeDamage(int damage, bool ignorePlotArmor = false)
    {
        // Should NOT be called if in card form.
        if (Type != CardType.Unit) return;

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

        if (OnTakeDamage != null)
        {
            foreach (Func<Task> handler in OnTakeDamage.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }

        if (CurrentPower == 0)
            await this.Destroy();
    }

    /// <summary>
    /// Method to grant Power to this card and return the amount granted. 
    /// </summary>
    /// <param name="powerAmount"></param>
    /// <returns></returns>
    public async Task GrantPower(int powerAmount)
    {
        MaxPower += powerAmount;
        CurrentPower += powerAmount;

        if (OnGrantPower != null)
        {
            foreach (Func<Task> handler in OnGrantPower.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    /// <summary>
    /// Method to grant Plot Armor to this card and return the amount granted.
    /// </summary>
    /// <param name="armorAmount"></param>
    /// <returns></returns>
    public async Task GrantPlotArmor(int armorAmount)
    {
        CurrentPlotArmor += armorAmount;

        if (OnGrantPlotArmor != null)
        {
            foreach (Func<Task> handler in OnGrantPlotArmor.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }
    }

    /// <summary>
    /// Method to heal this unit by aan amount up to the max power.
    /// </summary>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public async Task Heal(int healAmount)
    {
        // Should NOT be called if in card form.
        if (Type != CardType.Unit) return;

        // should never be negative
        int totalHealPossible = MaxPower - CurrentPower;

        if (totalHealPossible == 0) return;

        healAmount = Math.Min(totalHealPossible, healAmount);

        CurrentPower += healAmount;

        if (OnHeal != null)
        {
            foreach (Func<Task> handler in OnHeal.GetInvocationList()
                .Cast<Func<Task>>())
            {
                await handler();
            }
        }
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
    public async Task ApplyCondition(string conditionName, ICondition condition)
    {
        if (!conditions.ContainsKey(conditionName))
        {
            conditions.Add(conditionName, condition);
            await condition.OnAdd();
        }
        else
        {
            await conditions[conditionName].OnSurplus(condition);
        }
    }

    /// <summary>
    /// Method to remove a condition if possible
    /// </summary>
    /// <param name="conditionName"></param>
    public async Task RemoveCondition(string conditionName)
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
    public async Task TriggerCondition(string conditionName)
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


    // ----------------------------------------------------------------------------
    // Loading Assets & Overwriting Card Prefabs
    // ----------------------------------------------------------------------------

    /// <summary>
    /// Loads a sprite and returns it from a path string. 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(string filename)
    {
        if (string.IsNullOrEmpty(filename)) return null;

        string path = Path.Combine(Application.streamingAssetsPath, filename);

        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(100, 100, TextureFormat.RGB24, false);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        return null;
    }

    /// <summary>
    /// At instantiation will be used to overwrite placeholder card gameobject
    /// with correct sprites and initial values.
    /// </summary>
    public void OverwriteCardPrefab()
    {
        cardView = transform.Find("CardPrefab(Clone)");
        if (cardView == null)
            return;

        // Load portrait picture
        Sprite sprite = LoadSprite(PortraitPath);
        Transform portrait = cardView.Find("Portrait");

        if (sprite != null)
            portrait.GetComponent<Image>().sprite = sprite;

        cardTextCost = cardView.Find("Cost").GetComponent<TextMeshProUGUI>();
        cardTextCost.text = CurrentCost.ToString();

        // Placeholder has Unit Card frame automatically, so replace it if needed
        if (Type == CardType.Spell)
        {
            Sprite spellCardFrame = LoadSprite("SpellCardFrontFrame.png");
            Transform background = cardView.Find("Background");
            background.GetComponent<Image>().sprite = spellCardFrame;

            cardView.Find("Power").gameObject.SetActive(false);
            cardView.Find("PlotArmor").gameObject.SetActive(false);

        }
        else
        {
            cardTextPower = cardView.Find("Power").GetComponent<TextMeshProUGUI>();
            cardTextPower.text = CurrentPower.ToString();
            cardTextPlotArmor = cardView.Find("PlotArmor").GetComponent<TextMeshProUGUI>();
            cardTextPlotArmor.text = CurrentPlotArmor.ToString();

        }

        cardView.Find("Name").GetComponent<TextMeshProUGUI>().text = Title;
        cardView.Find("Description").GetComponent<TextMeshProUGUI>().text = Description;

        if (IsHidden)
            cardView.Find("Cardback").gameObject.SetActive(true);
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
        Sprite sprite = LoadSprite(PortraitPath);
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
            cardTextPower.text = CurrentPower.ToString();
        if (cardTextPlotArmor)
            cardTextPlotArmor.text = CurrentPlotArmor.ToString();
    }

    /// <summary>
    /// Updates the unit stat text on the unit view.
    /// </summary>
    private void UpdateUnitStatText()
    {
        if (!unitView) return;
        unitTextPower.text = CurrentPower.ToString();
        unitTextPlotArmor.text = CurrentPlotArmor.ToString();
    }
}
