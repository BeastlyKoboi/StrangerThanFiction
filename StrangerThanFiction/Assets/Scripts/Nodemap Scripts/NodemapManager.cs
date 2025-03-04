using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodemapManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private ScrollRect mapScrollRect;
    [SerializeField] private GameObject bookcaseContent;
    [SerializeField] private NodeMenu nodeMenu;

    [SerializeField] private GameObject bookNodePrefab;
    [SerializeField] private GameObject emptyShelfPrefab;
    public SpecialNodeData[] specialNodeDatas;
    public BattleNodeData[] battleNodeDatas;

    public List<List<MapNode>> mapNodes;

    public MapNode selectedNode;

    public MapNode CreateNode(NodeData data, GameObject parent)
    {
        GameObject nodeObj = Instantiate(bookNodePrefab, Vector3.zero, Quaternion.identity, parent.transform);
        MapNode mapNode = nodeObj.GetComponent<MapNode>();
        mapNode.Initialize(data);
        return mapNode;
    }

    public void CreateNodeMap()
    {
        int numModules = 3;

        mapNodes = new List<List<MapNode>>();

        for (int i = 0; i < numModules; i++)
        {
            mapNodes.Add(new List<MapNode>());
            mapNodes.Add(new List<MapNode>());
        }

        // creates nodes bottom to top, left to right
        for (int i = 0; i < mapNodes.Count; i += 2)
        {
            GameObject bookshelf;
            GameObject bookrow;

            MapNode specialNode;
            SpecialNodeData specialNodeData;
            MapNode battleNode;
            BattleNodeData battleNodeData;

            // Special Nodes
            // create bookshelf
            bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
            bookrow = bookshelf.transform.Find("BookRow").gameObject;

            specialNodeData = specialNodeDatas[Random.Range(0, specialNodeDatas.Length)];
            specialNode = CreateNode(specialNodeData, bookrow);
            specialNode.ToggleSelectable(false);
            mapNodes[i].Add(specialNode);

            specialNodeData = specialNodeDatas[Random.Range(0, specialNodeDatas.Length)];
            specialNode = CreateNode(specialNodeData, bookrow);
            specialNode.ToggleSelectable(false);
            mapNodes[i].Add(specialNode);

            // Battle nodes
            bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
            bookrow = bookshelf.transform.Find("BookRow").gameObject;

            battleNodeData = battleNodeDatas[Random.Range(0, battleNodeDatas.Length)];
            battleNode = CreateNode(battleNodeData, bookrow);
            battleNode.ToggleSelectable(false);
            mapNodes[i + 1].Add(battleNode);

            battleNodeData = battleNodeDatas[Random.Range(0, battleNodeDatas.Length)];
            battleNode = CreateNode(battleNodeData, bookrow);
            battleNode.ToggleSelectable(false);
            mapNodes[i + 1].Add(battleNode);


        }

        // connect nodes
        for (int i = 0; i < mapNodes.Count; i++)
        {
            for (int j = 0; j < mapNodes[i].Count; j++)
            {
                if (i + 1 < mapNodes.Count)
                {
                    foreach (MapNode node in mapNodes[i + 1]) 
                    {
                        mapNodes[i][j].AddNeighbor(node);

                    }
                }
            }
        }

        foreach (MapNode node in mapNodes[0])
        {
            node.SetOnClick(SelectNewNode);
            node.ToggleSelectable(true);
        }

        mapScrollRect.verticalNormalizedPosition = 0;
    }

    public void SelectNewNode(MapNode selectedNode)
    {
        foreach (List<MapNode> nodeList in mapNodes)
        {
            foreach (MapNode node in nodeList)
            {
                node.ToggleSelectable(false);
                node.ResetOnClick();
            }
        }

        this.selectedNode = selectedNode;

        nodeMenu.OpenMenu(selectedNode);
    }

    public void SetSelectableNodes()
    {
        foreach (MapNode node in selectedNode.GetNeighbors())
        {
            node.SetOnClick(SelectNewNode);
            node.ToggleSelectable(true);
        }
    }

    public void LoadData(GameData data)
    {

        //if (data.)

        CreateNodeMap();

        CardFactory.Instance.Initialize();

        //throw new System.NotImplementedException();
    }

    public void SaveData(GameData data)
    {
        if (selectedNode != null && selectedNode.GetNodeData() is BattleNodeData)
            data.nextBattleNode = (BattleNodeData)selectedNode.GetNodeData();

        //throw new System.NotImplementedException();
    }

}
