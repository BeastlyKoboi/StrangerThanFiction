using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Binding : MonoBehaviour, IDataPersistence, IDamagable
{
    public BattleNodeData battleNodeData;

    public int BindingPower { get; private set; }
    public int BindingDamage { get; private set; }

    public UniTaskEvent<BindingState> OnBindingChange = new UniTaskEvent<BindingState>();
    public UniTaskEvent<DamageData> OnTakeDamage = new UniTaskEvent<DamageData>();
    public UniTaskEvent<DamageData> OnHeal = new UniTaskEvent<DamageData>();

    [SerializeField] private CombatManager combatManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject popupWindow;

    public void LoadData(GameData data)
    {
        battleNodeData = data.nextBattleNode;
        BindingPower = data.bindingPower;
    }

    public void SaveData(GameData data)
    {
        
    }

    private void Start()
    {
        uiManager.UpdateBinding(new BindingState(battleNodeData, BindingPower, 0, 0));
        uiManager.UpdateBindingWindow(new BindingState(battleNodeData, BindingPower, 0, 0));
    }

    public async UniTask TakeDamage(DamageData damageData)
    {
        // first apply special conditions that may modify the damage


        // Then check if the damage's source is an ally or enemy
        int prevBindingDamage;
        if (damageData.source is CardModel card && card.Owner == combatManager.player2)
        {
            // then apply the healing 
            prevBindingDamage = BindingDamage;
            BindingDamage -= damageData.damage / 2;
        } else
        {
            // then apply the damage
            prevBindingDamage = BindingDamage;
            BindingDamage += damageData.damage;
        }

        await OnBindingChange.InvokeAsync(new BindingState(battleNodeData, BindingPower, prevBindingDamage, BindingDamage));
        await OnTakeDamage.InvokeAsync(damageData);

        uiManager.UpdateBinding(new BindingState(
            battleNodeData,
            bindingPower: BindingPower, 
            prevTotalBindingDamage: prevBindingDamage, 
            currTotalBindingDamage: BindingDamage));
    }
}