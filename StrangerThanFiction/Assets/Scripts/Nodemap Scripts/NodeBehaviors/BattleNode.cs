using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleNode : MonoBehaviour, IDataPersistence
{
    private BattleNodeData _battleNodeData;
    private GameData gameData;
    private int _binding;
    private CombatHover combatHover;
    private GameObject _selectNodeObj;
    private MapNode mapNode; 
    private SceneLoader sceneLoader;

    public void Initialize(BattleNodeData nodeData, GameData gameData, FlatNode flatNode = null)
    {
        _battleNodeData = nodeData;
        this.gameData = gameData;
        _selectNodeObj = transform.Find("Seal").gameObject;
        combatHover = _selectNodeObj.AddComponent<CombatHover>();
        combatHover.SetBattleNode(this);
        mapNode = GetComponent<MapNode>();

        mapNode.AddOnClick((MapNode mapNode) => { 
            gameData.combatResults = null;
            gameData.nextBattleNode = _battleNodeData.name;
            gameData.bindingPower = _binding;
            sceneLoader = FindObjectOfType<SceneLoader>();
            sceneLoader.LoadScene("Gameplay");
        });

        if (flatNode != null)
            SetBinding(flatNode.flatNodeBattle.binding);
    }

    public void SetBinding(int newBinding)
    {
        _binding = newBinding;
    }

    public int GetBinding()
    {
        return _binding;
    }

    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(GameData data)
    {
        
    }

    public FlatNodeBattle GetFlatNodeBattle()
    {
        return new FlatNodeBattle
        {
            binding = _binding
        };
    }
}
