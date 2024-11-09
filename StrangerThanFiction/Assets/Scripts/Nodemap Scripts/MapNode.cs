using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNodeFlat
{
    public string Title;
    public Vector3 Pos;
    public List<string> neighbors;
}


public class MapNode : MonoBehaviour
{
    private NodeData _nodeData;
    private List<MapNode> _neighbors;

    private bool _isSelectable;
    private GameObject _selectNodeObj;
    private Button _selectNodeBtn;

    public void Initialize(NodeData data)
    {
        _nodeData = data;
        _neighbors = new List<MapNode>();

        _selectNodeObj = transform.Find("SelectNodeBtn").gameObject;
        _selectNodeObj.GetComponent<Image>().sprite = _nodeData.Icon;

        _selectNodeBtn = _selectNodeObj.GetComponent<Button>();

        if (_nodeData is BattleNodeData)
        {
            BattleNodeData battleNodeData = (BattleNodeData)_nodeData;
            _selectNodeObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = battleNodeData.Title;
        }
        else if (_nodeData is SpecialNodeData)
        {
            SpecialNodeData specialNodeData = (SpecialNodeData)_nodeData;
            _selectNodeObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = specialNodeData.Title;
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
        if (_isSelectable == isSelectable)
            return;

        _isSelectable = isSelectable;

        if (_isSelectable)
            _selectNodeObj.AddComponent<NodePulse>();
        else
            Destroy(_selectNodeObj.GetComponent<NodePulse>());
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

}
