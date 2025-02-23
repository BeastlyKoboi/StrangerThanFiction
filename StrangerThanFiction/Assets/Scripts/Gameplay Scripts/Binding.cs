using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Binding : MonoBehaviour, IDataPersistence, IDamagable
{
    public int BindingPower { get; private set; }
    public int BindingDamage { get; private set; }

    public UniTaskEvent<DamageData> OnTakeDamage = new UniTaskEvent<DamageData>();
    public UniTaskEvent<DamageData> OnHeal = new UniTaskEvent<DamageData>();

    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;

    public void LoadData(GameData data)
    {
        BindingPower = data.bindingPower;
    }

    public void SaveData(GameData data)
    {
        
    }

    private void Start()
    {
        uiManager.UpdateBinding(new BindingState(BindingPower, 0, 0));
    }

    public async UniTask TakeDamage(DamageData damageData)
    {
        // first apply special conditions that may modify the damage


        // Then check if the damage's source is an ally or enemy
        int prevBindingDamage;
        if (damageData.source is CardModel card && card.Owner == gameManager.player2)
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

        await OnTakeDamage.InvokeAsync(damageData);

        uiManager.UpdateBinding(new BindingState(
            bindingPower: BindingPower, 
            prevTotalBindingDamage: prevBindingDamage, 
            currTotalBindingDamage: BindingDamage));
    }
}