using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PathAlgorithm
{
    AStar,
    BFS,
    DFS,
    Dijkstra,
}

public interface IPathFinder
{
    void Init(int w, int h);

    void ResetMap();

    E_Node_Type GetNodeType(int x, int y);

    List<Vector2Int> FindPathPositions(
        Vector2Int start,
        Vector2Int end);
}

public class TestPathFinding : MonoBehaviour
{

    public PathAlgorithm algorithm;
    private IPathFinder manager;

    public int beginX;
    public int beginY;
    public int offsetX;
    public int offsetY;
    public int mapW;
    public int mapH;

    public Material Red;
    public Material Yellow;
    public Material Green;
    private Material defaultMaterial;

    public Button restart;

    private Dictionary<string, GameObject> nodeObjects = new Dictionary<string, GameObject>();
    private Vector2Int beginPos = Vector2Int.right * -1;
    private Vector2Int endPos = Vector2Int.right * -1;

    void Start()
    {
        switch (algorithm)
        {
            case PathAlgorithm.AStar:
                manager = AStarManager.Instance;
                break;
            case PathAlgorithm.BFS:
                manager = BFSManager.Instance;
                break;
            case PathAlgorithm.DFS:
                manager = DFSManager.Instance;
                break;
            case PathAlgorithm.Dijkstra:
                manager = DijkstraManager.Instance;
                break;
        }
        manager.Init(mapW, mapH);

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (defaultMaterial == null)
                    defaultMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);

                obj.name = i + "_" + j;
                nodeObjects.Add(obj.name, obj);

                if (manager.GetNodeType(i,j) == E_Node_Type.Stop)
                {
                    obj.GetComponent<MeshRenderer>().material = Red;
                }
            }
        }

        restart.onClick.AddListener(ResetMap);
    }

    /// <summary>
    /// 重置地图
    /// </summary>
    public void ResetMap()
    {
        beginPos = Vector2Int.right * -1;
        endPos = Vector2Int.right * -1;
        if (nodeObjects.Count == 0)
            return;
        manager.ResetMap();

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                nodeObjects[i + "_" + j].GetComponent<MeshRenderer>().material =
                    manager.GetNodeType(i,j) == E_Node_Type.Stop ? Red : defaultMaterial;
            }
        }
    }

    private void Update()
    {
        //如果点到了UI元素上 或者已经选择了终点 则不进行射线检测
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() || endPos != (Vector2Int.right * -1))
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //进行射线检测
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out info, 1000))
            {
                if (beginPos == Vector2Int.right * -1)
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    beginPos = new Vector2Int(int.Parse(strs[0]), int.Parse(strs[1]));
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = Yellow;
                }
                else
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    endPos = new Vector2Int(int.Parse(strs[0]), int.Parse(strs[1]));

                    //寻路
                    List<Vector2Int> list = manager.FindPathPositions(beginPos, endPos);
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            nodeObjects[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = Green;
                        }
                    }
                }

            }
        }
    }
}
