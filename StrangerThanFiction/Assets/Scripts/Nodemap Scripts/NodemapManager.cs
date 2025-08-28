using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NodemapManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private RunInfo runInfo;
    private GameData gameData;

    [SerializeField] private ScrollRect mapScrollRect;
    [SerializeField] private GameObject bookcaseContent;
    [SerializeField] private NodeMenu nodeMenu;
    [SerializeField] private GameOverMenu gameOverMenu;

    [SerializeField] private GameObject bookNodePrefab;
    [SerializeField] private GameObject emptyShelfPrefab;

    [Header("Node Data")]
    public SpecialNodeData sheherazadeNodeData;
    public SpecialNodeData[] specialNodeDatas;
    public BattleNodeData[] battleNodeDatas;
    public BattleNodeData[] bossBattleNodeDatas;
    [SerializeField] private BattleNodeDictionary battleNodeDictionary;
    [SerializeField] private SpecialNodeDictionary specialNodeDictionary;

    public List<List<MapNode>> mapNodes;

    public MapNode selectedNode;

    public MapNode CreateNode(NodeData nodeData, GameObject parent, FlatNode flatNode = null)
    {
        GameObject nodeObj = Instantiate(bookNodePrefab, Vector3.zero, Quaternion.identity, parent.transform);
        MapNode mapNode = nodeObj.GetComponent<MapNode>();
        mapNode.Initialize(nodeData, gameData, flatNode);
        return mapNode;
    }

    public void CreateNodeMap()
    {
        int baseBinding = 20;
        mapNodes = new List<List<MapNode>>();
        List<MapNode> startingSelectableNodes = new List<MapNode>();
        bool isMapCompleted = false;

        FlatNodeRow[] flatNodeMap = gameData.GetRunData().nodeMap.flatNodeMap;

        if (flatNodeMap == null || flatNodeMap.Length == 0)
        {
            int numModules = 5;

            mapNodes.Add(new List<MapNode>());
            CreateSpecialNodes(0, 1, sheherazadeNodeData);

            for (int i = 0; i < numModules; i++)
            {
                mapNodes.Add(new List<MapNode>());
                mapNodes.Add(new List<MapNode>());
            }

            for (int i = 1; i < mapNodes.Count; i += 2)
            {
                CreateSpecialNodes(i, 2);
                baseBinding = (int)(baseBinding * 1.2);

                if (i + 2 < mapNodes.Count) 
                    CreateBattleNodes(i + 1, baseBinding, numNodes: Random.Range(2, 5));
                else
                    CreateBattleNodes(i + 1, baseBinding, 
                        numNodes: 1, 
                        specificNodeData: bossBattleNodeDatas[Random.Range(0, bossBattleNodeDatas.Length)]);
            }

            startingSelectableNodes = mapNodes[0];
        }
        else
        {
            for (int i = 0; i < flatNodeMap.Length; i++)
            {
                mapNodes.Add(new List<MapNode>());
                GameObject bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
                GameObject bookrow = bookshelf.transform.Find("BookRow").gameObject;

                for (int j = 0; j < flatNodeMap[i].flatNodesArr.Length; j++)
                {
                    FlatNode nodeFlat = flatNodeMap[i].flatNodesArr[j];
                    NodeData nodeData = battleNodeDictionary.GetByKey(nodeFlat.nodeDataKey) as NodeData ??
                       specialNodeDictionary.GetByKey(nodeFlat.nodeDataKey) as NodeData;

                    MapNode baseNode = CreateNode(nodeData, bookrow, nodeFlat);
                    baseNode.ToggleSelectable(nodeFlat.isSelectable);
                    mapNodes[i].Add(baseNode);

                    if (nodeFlat.isSelectable)
                    {
                        startingSelectableNodes.Add(baseNode);
                    }

                    if (nodeFlat.isSelected && nodeData is BattleNodeData battleNodeData && gameData.GetRunData().combatResults.victory)
                    {
                        if (!gameData.hasCompletedTutorial)
                            gameData.hasCompletedTutorial = true;

                        if (battleNodeData.Type == BattleNodeType.Boss)
                            isMapCompleted = true;

                        startingSelectableNodes.Remove(baseNode);
                        baseNode.ToggleSelectable(false);

                        if (gameData.GetRunData().combatResults.victory && flatNodeMap.GetLength(0) > i + 1)
                        {
                            foreach (FlatNode neighbor in flatNodeMap[i + 1].flatNodesArr)
                            {
                                neighbor.isSelectable = true;
                            }
                        }

                        runInfo.AddCurrency(gameData.GetRunData().combatResults.gainedDeus).Forget();

                    }
                }
            }
        }

        ConnectNodes();
        InitializeSelectableNodes(startingSelectableNodes);


        for (int i = 0; i < mapNodes.Count; i++)
        {
            bool hasSelectableNode = false;

            foreach (MapNode node in mapNodes[i])
            {
                if (startingSelectableNodes.Contains(node))
                {
                    hasSelectableNode = true;
                    break;
                }
            }

            if (hasSelectableNode)
            {
                mapScrollRect.verticalNormalizedPosition = Mathf.Clamp((float)(i + 1f) / (float)(mapNodes.Count - 1), 0f, 1f);
                break;
            }
        }

        if (isMapCompleted) 
        {
            gameOverMenu.ToggleGameOverMenu(true);
            gameData.GetRunData().runHasEnded = true;
            DataPersistenceManager.instance.DeleteGame();
        }

    }

    private void CreateSpecialNodes(int index, int numNodes, SpecialNodeData specificNodeData = null)
    {
        GameObject bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
        GameObject bookrow = bookshelf.transform.Find("BookRow").gameObject;

        for (int j = 0; j < numNodes; j++)
        {
            SpecialNodeData specialNodeData;
            if (specificNodeData != null)
                specialNodeData = specificNodeData;
            else
                specialNodeData = specialNodeDatas[Random.Range(0, specialNodeDatas.Length)];
            MapNode baseNode = CreateNode(specialNodeData, bookrow);
            baseNode.ToggleSelectable(false);
            mapNodes[index].Add(baseNode);
        }
    }

    private void CreateBattleNodes(int index, int binding, int numNodes = 2, BattleNodeData specificNodeData = null)
    {
        GameObject bookshelf = Instantiate(emptyShelfPrefab, Vector3.zero, Quaternion.identity, bookcaseContent.transform);
        GameObject bookrow = bookshelf.transform.Find("BookRow").gameObject;

        for (int j = 0; j < numNodes; j++)
        {
            BattleNodeData battleNodeData;
            if (specificNodeData == null) 
                battleNodeData = battleNodeDatas[Random.Range(0, battleNodeDatas.Length)];
            else
                battleNodeData = specificNodeData;

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


        if (this.selectedNode.GetComponent<BattleNode>() == null)
        {
            foreach (MapNode node in selectedNode.GetNeighbors())
            {
                node.AddOnClick(SelectNewNode);
                node.ToggleSelectable(true);
            }
        }
       
    }
    
    public void LoadData(GameData data)
    {
        gameData = data;

        CreateNodeMap();

        CardFactory.Instance.Initialize();

        //Time.timeScale = 4f;
    }

    public void SaveData(GameData data)
    {
        data.GetRunData().nodeMap = new FlatNodeMap();
        data.GetRunData().nodeMap.flatNodeMap = new FlatNodeRow[mapNodes.Count];

        for (int i = 0; i < mapNodes.Count; i++)
        {
            data.GetRunData().nodeMap.flatNodeMap[i] = new FlatNodeRow();
            data.GetRunData().nodeMap.flatNodeMap[i].flatNodesArr = new FlatNode[mapNodes[i].Count];
            for (int j = 0; j < mapNodes[i].Count; j++)
            {
                data.GetRunData().nodeMap.flatNodeMap[i].flatNodesArr[j] = mapNodes[i][j].GetFlattenedNode();

                if (selectedNode == mapNodes[i][j])
                {
                    data.GetRunData().nodeMap.flatNodeMap[i].flatNodesArr[j].isSelected = true;
                }
            }
        }

        Debug.Log(data.GetRunData().nextBattleNode);

    }

}
