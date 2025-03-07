using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    private NodeData _nodeData;
    private List<MapNode> _neighbors;
    
    private bool _isSelectable;
    private GameObject _selectNodeObj;
    private Button _selectNodeBtn;

    private NodePulse nodePulse; 

    public void Initialize(NodeData data, GameData gameData, int binding = 0)
    {
        _nodeData = data;
        _neighbors = new List<MapNode>();


        _selectNodeObj = transform.Find("Seal").gameObject;
        _selectNodeObj.transform.Find("Icon").GetComponent<Image>().sprite = _nodeData.Icon;

        _selectNodeBtn = _selectNodeObj.GetComponent<Button>();
        
        transform.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = _nodeData.Title;


        if (_nodeData is BattleNodeData)
        {
            BattleNodeData battleNodeData = (BattleNodeData)_nodeData;

            BattleNode battleNode = gameObject.AddComponent<BattleNode>();

            battleNode.Initialize(battleNodeData, gameData, binding);

        }
        else if (_nodeData is SpecialNodeData)
        {
            SpecialNodeData specialNodeData = (SpecialNodeData)_nodeData;
            Type nodeType = Type.GetType(specialNodeData.name);
            if (nodeType == null)
            {
                Debug.LogError("Node type not found: " + _nodeData.name);
                return;
            }
            gameObject.AddComponent(nodeType);
        }

    }

    public void AddNeighbor(MapNode node)
    {
        if (!_neighbors.Contains(node))
        {
            _neighbors.Add(node);
        }
    }

    public List<MapNode> GetNeighbors()
    {
        return _neighbors;
    }

    public NodeData GetNodeData()
    {
        return _nodeData;
    }

    public void OnSelected()
    {
        Debug.Log("Clicked on node: " + _nodeData.Title);
    }

    public void ToggleSelectable(bool isSelectable)
    {
        if (isSelectable && nodePulse == null)
        {
            nodePulse = _selectNodeObj.AddComponent<NodePulse>();

        }
        else if (!isSelectable && nodePulse != null)
        {
            Destroy(nodePulse);
            nodePulse = null;
        }

        _isSelectable = isSelectable;
        _selectNodeBtn.interactable = isSelectable;
    }

    public void ResetOnClick()
    {
        _selectNodeBtn.onClick.RemoveAllListeners();
    }

    public void SetOnClick(Action<MapNode> action)
    {
        _selectNodeBtn.onClick.RemoveAllListeners();
        _selectNodeBtn.onClick.AddListener(() => { action(this); });
    }

    public void AddOnClick(Action<MapNode> action)
    {
        _selectNodeBtn.onClick.AddListener(() => { action(this); });
    }

    public void RemoveOnClick(Action<MapNode> action)
    {
        _selectNodeBtn.onClick.RemoveListener(() => { action(this); });
    }

    public FlatNode GetFlattenedNode()
    {
        return new FlatNode
        {
            Title = _nodeData.Title,
            isSelectable = _isSelectable,
            nodeData = _nodeData,
            isSelected = false
        };
    }

}
