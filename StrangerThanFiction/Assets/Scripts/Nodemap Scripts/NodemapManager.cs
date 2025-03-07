using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NodemapManager : MonoBehaviour, IDataPersistence
{
    private GameData gameData;

    [SerializeField] private ScrollRect mapScrollRect;
    [SerializeField] private GameObject bookcaseContent;
    [SerializeField] private NodeMenu nodeMenu;

    [SerializeField] private GameObject bookNodePrefab;
    [SerializeField] private GameObject emptyShelfPrefab;
    public SpecialNodeData[] specialNodeDatas;
    public BattleNodeData[] battleNodeDatas;

    public List<List<MapNode>> mapNodes;

    public MapNode selectedNode;

    public MapNode CreateNode(NodeData nodeData, GameObject parent)
    {
        GameObject nodeObj = Instantiate(bookNodePrefab, Vector3.zero, Quaternion.identity, parent.transform);
        MapNode mapNode = nodeObj.GetComponent<MapNode>();
        mapNode.Initialize(nodeData, gameData);
        return mapNode;
    }

    public void CreateNodeMap()
    {
        int baseBinding = 20;
        mapNodes = new List<List<MapNode>>();
        List<MapNode> startingSelectableNodes = new List<MapNode>();

        if (gameData.nodeMap.flatNodeMap == null || gameData.nodeMap.flatNodeMap.Length == 0)
        {
            int numModules = 3;

            for (int i = 0; i < numModules; i++)
            {
                mapNodes.Add(new List<MapNode>());
                mapNodes.Add(new List<MapNode>());
            }

            for (int i = 0; i < mapNodes.Count; i += 2)
            {
                CreateSpecialNodes(i);
                baseBinding = (int)(baseBinding * 1.2);
                CreateBattleNodes(i + 1, baseBinding);
            }
            
            startingSelectableNodes = mapNodes[0];
        }
        else
        {
            for (int i = 0; i < gameData.nodeMap.flatNodeMap.Length; i++)
            {
                mapNodes.Add(new List<MapNode>());
                GameObject bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
                GameObject bookrow = bookshelf.transform.Find("BookRow").gameObject;

                for (int j = 0; j < gameData.nodeMap.flatNodeMap[i].flatNodesArr.Length; j++)
                {
                    FlatNode nodeFlat = gameData.nodeMap.flatNodeMap[i].flatNodesArr[j];

                    MapNode baseNode = CreateNode(nodeFlat.nodeData, bookrow);
                    baseNode.ToggleSelectable(nodeFlat.isSelectable);
                    mapNodes[i].Add(baseNode);

                    if (nodeFlat.isSelectable)
                    {
                        startingSelectableNodes.Add(baseNode);
                    }

                    if (nodeFlat.isSelected && nodeFlat.nodeData is BattleNodeData && gameData.combatResults.victory)
                    {
                        startingSelectableNodes.Remove(baseNode);
                        baseNode.ToggleSelectable(false);
                    }
                }
            }
        }

        ConnectNodes();
        InitializeSelectableNodes(startingSelectableNodes);
        mapScrollRect.verticalNormalizedPosition = 0;
    }

    private void CreateSpecialNodes(int index, SpecialNodeData specialNodeData = null)
    {
        GameObject bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
        GameObject bookrow = bookshelf.transform.Find("BookRow").gameObject;

        for (int j = 0; j < 2; j++)
        {
            if (specialNodeData == null)
                specialNodeData = specialNodeDatas[Random.Range(0, specialNodeDatas.Length)];
            MapNode baseNode = CreateNode(specialNodeData, bookrow);
            baseNode.ToggleSelectable(false);
            mapNodes[index].Add(baseNode);
        }
    }

    private void CreateBattleNodes(int index, int binding, BattleNodeData battleNodeData = null)
    {
        GameObject bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
        GameObject bookrow = bookshelf.transform.Find("BookRow").gameObject;

        for (int j = 0; j < 2; j++)
        {
            if (battleNodeData == null)
                battleNodeData = battleNodeDatas[Random.Range(0, battleNodeDatas.Length)];
            MapNode baseNode = CreateNode(battleNodeData, bookrow);
            baseNode.ToggleSelectable(false);

            BattleNode battleNode = baseNode.GetComponent<BattleNode>();
            battleNode.SetBinding(binding);
            mapNodes[index].Add(baseNode);
        }
    }

    private void ConnectNodes()
    {
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
    }

    private void InitializeSelectableNodes(List<MapNode> selectableNodes)
    {
        foreach (MapNode node in selectableNodes)
        {
            node.AddOnClick(SelectNewNode);
            node.ToggleSelectable(true);
        }
    }


    public void SelectNewNode(MapNode selectedNode)
    {
        foreach (List<MapNode> nodeList in mapNodes)
        {
            foreach (MapNode node in nodeList)
            {
                if (node != selectedNode)
                {
                    node.ToggleSelectable(false);
                    node.RemoveOnClick(SelectNewNode);
                }
            }
        }

        this.selectedNode = selectedNode;
        this.selectedNode.RemoveOnClick(SelectNewNode);

        foreach (MapNode node in selectedNode.GetNeighbors())
        {
            node.AddOnClick(SelectNewNode);
            node.ToggleSelectable(true);
        }
    }
    
    public void LoadData(GameData data)
    {

        //if (data.)

        gameData = data;

        CreateNodeMap();

        CardFactory.Instance.Initialize();

        //throw new System.NotImplementedException();
    }

    public void SaveData(GameData data)
    {
        Debug.Log("Saving node map");

        if (selectedNode != null && selectedNode.GetNodeData() is BattleNodeData)
        {
            data.nextBattleNode = (BattleNodeData)selectedNode.GetNodeData();

            data.player2Deck.deckEntries.Clear();

            foreach (DeckEntry entry in data.nextBattleNode.DeckInventory.deckEntries)
            {
                data.player2Deck.deckEntries.Add(new DeckEntry(entry.cardName, entry.numCopies));
            }
        }

        // save the nodemap to the data
        

        data.nodeMap = new FlatNodeMap();
        data.nodeMap.flatNodeMap = new FlatNodeRow[mapNodes.Count];

        for (int i = 0; i < mapNodes.Count; i++)
        {
            Debug.Log("Inside First for loop");
            data.nodeMap.flatNodeMap[i] = new FlatNodeRow();
            data.nodeMap.flatNodeMap[i].flatNodesArr = new FlatNode[mapNodes[i].Count];
            for (int j = 0; j < mapNodes[i].Count; j++)
            {
                Debug.Log("Inside second for loop");
                data.nodeMap.flatNodeMap[i].flatNodesArr[j] = mapNodes[i][j].GetFlattenedNode();

                if (selectedNode == mapNodes[i][j])
                {
                    data.nodeMap.flatNodeMap[i].flatNodesArr[j].isSelected = true;
                    Debug.Log("Selected node saved. ");
                }
            }
        }

        Debug.Log(data.nodeMap);


        //throw new System.NotImplementedException();
    }

}
