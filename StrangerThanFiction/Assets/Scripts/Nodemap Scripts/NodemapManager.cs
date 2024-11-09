using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodemapManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject nodeParent;
    [SerializeField] private NodeMenu nodeMenu;

    public GameObject simpleNodePrefab;
    public GameObject arrowPrefab;
    public SpecialNodeData[] specialNodeDatas;
    public BattleNodeData[] battleNodeDatas;

    public List<List<MapNode>> mapNodes;

    public MapNode selectedNode;


    public MapNode CreateNode(NodeData data, Vector3 pos)
    {
        GameObject nodeObj = Instantiate(simpleNodePrefab, pos, Quaternion.identity, nodeParent.transform);
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
            MapNode specialNode;
            SpecialNodeData specialNodeData;
            MapNode battleNode;
            BattleNodeData battleNodeData;

            int spacing = 250;

            specialNodeData = specialNodeDatas[Random.Range(0, specialNodeDatas.Length)];
            specialNode = CreateNode(specialNodeData, startPoint.transform.position + new Vector3(-200, i * spacing, 0));
            specialNode.ToggleSelectable(false);
            mapNodes[i].Add(specialNode);

            specialNodeData = specialNodeDatas[Random.Range(0, specialNodeDatas.Length)];
            specialNode = CreateNode(specialNodeData, startPoint.transform.position + new Vector3(200, i * spacing, 0));
            specialNode.ToggleSelectable(false);
            mapNodes[i].Add(specialNode);

            battleNodeData = battleNodeDatas[Random.Range(0, battleNodeDatas.Length)];
            battleNode = CreateNode(battleNodeData, startPoint.transform.position + new Vector3(-200, (i + 1) * spacing, 0));
            battleNode.ToggleSelectable(false);
            mapNodes[i + 1].Add(battleNode);

            battleNodeData = battleNodeDatas[Random.Range(0, battleNodeDatas.Length)];
            battleNode = CreateNode(battleNodeData, startPoint.transform.position + new Vector3(200, (i + 1) * spacing, 0));
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

        //throw new System.NotImplementedException();
    }

    public void SaveData(GameData data)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
