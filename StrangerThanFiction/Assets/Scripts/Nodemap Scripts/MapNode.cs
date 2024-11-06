using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    private MapNodeData _nodeData;
    private List<MapNode> _neighbors;

    private bool _isSelectable;
    private GameObject _selectNodeBtn;

    public void Initialize(MapNodeData data)
    {
        _nodeData = data;
        _neighbors = new List<MapNode>();

        _selectNodeBtn = transform.Find("SelectNodeBtn").gameObject;


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
            Destroy(GetComponent<NodePulse>());
        else
            _selectNodeBtn.AddComponent<NodePulse>();
    }

}
