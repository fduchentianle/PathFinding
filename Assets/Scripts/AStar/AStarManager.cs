using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 八方向A*寻路管理器
/// </summary>
public class AStarManager : BaseManager<AStarManager>, IPathFinder
{
    //地图的宽高
    private int mapW;
    private int mapH;

    //地图的所有格子
    public AStarNode[,] nodes;
    //开启列表
    private List<AStarNode> openList;

    private AStarManager() { }

    /// <summary>
    /// 初始化地图信息
    /// </summary>
    public void Init(int w, int h)
    {
        if (w <= 0 || h <= 0)
        {
            Debug.Log("地图宽或高是负数，无法初始化");
            return;
        }

        mapW = w;
        mapH = h;
        nodes = new AStarNode[w, h];
        openList = new List<AStarNode>();

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
                nodes[i, j] = node;
            }
        }

        //预加载每个格子周围的可行走格子信息
        BuildLinks();
    }

    /// <summary>
    /// 寻路方法
    /// </summary>
    /// <param name="startPos">起始坐标</param>
    /// <param name="endPos">目标坐标</param>
    /// <returns></returns>
    public List<AStarNode> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        //如果传入的格子在地图外 则直接返回null
        if (startPos.x < 0 || startPos.x >= mapW || 
            startPos.y < 0 || startPos.y >= mapH ||
            endPos.x < 0 || endPos.x >= mapW ||
            endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("开始点坐标或目标点坐标在地图范围外");
            return null;
        }

        AStarNode start = nodes[startPos.x, startPos.y];
        AStarNode end = nodes[endPos.x, endPos.y];

        //如果传入的格子是障碍物 则直接返回null
        if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
        {
            Debug.Log("开始点坐标或目标点坐标是障碍物");
            return null;
        }

        //如果起始点和终点重合 则直接返回这个格子
        if (startPos.x == endPos.x && startPos.y == endPos.y)
        {
            Debug.Log("起点就是终点");
            start.father = null;
            start.f = start.g = start.h = 0;
            return new List<AStarNode>() { start };
        }

        //清空开启列表
        openList.Clear();

        //把起始格子加入关闭列表
        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        start.state = E_Search_State.Close;

        while (true)
        {
            foreach (NodeLink link in start.links)
            {
                AStarNode node = link.node;

                if (node.state == E_Search_State.Close)
                    continue;

                float newG = start.g + link.cost;

                if (node.state == E_Search_State.None || newG < node.g)
                {
                    node.father = start;
                    node.g = newG;
                    node.h = Mathf.Abs(end.x - node.x)
                           + Mathf.Abs(end.y - node.y);
                    node.f = node.g + node.h;

                    if (node.state == E_Search_State.None)
                    {
                        node.state = E_Search_State.Open;
                        openList.Add(node);
                    }
                }
            }

            if (openList.Count == 0)
            {
                Debug.Log("此为死路，无法找到路径");
                return null;
            }

            //选取开启列表中 寻路消耗最小的点
            //放入关闭列表中 然后再从开启列表中移除
            start = FindLowestNode();
            start.state = E_Search_State.Close;


            //如果这个点已经是终点了 寻路结束
            if (start.x == end.x && start.y == end.y)
            {
                List<AStarNode> path = new List<AStarNode>();
                path.Add(end);
                while (end.father != null)
                {
                    path.Add(end.father);
                    end = end.father;
                }
                path.Reverse();
                return path;
            }
        }
    }

    /// <summary>
    /// 重置地图
    /// </summary>
    public void ResetMap()
    {
        openList.Clear();

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                nodes[i,j].type = Random.Range(0,100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk;
                nodes[i,j].state = E_Search_State.None;
            }
        }

        //预加载每个格子周围的可行走格子信息
        BuildLinks();
    }


    private AStarNode FindLowestNode()
    {
        int minIndex = 0;

        for (int i = 0; i < openList.Count; i++)
        {
            if (openList[i].f <= openList[minIndex].f)
                minIndex = i;
        }

        AStarNode node = openList[minIndex];

        openList[minIndex] = openList[openList.Count - 1];
        openList.RemoveAt(openList.Count - 1);
        return node;
    }

    private void BuildLinks()
    {
        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                AStarNode node = nodes[i, j];
                node.links.Clear();
                if (node.type == E_Node_Type.Stop)
                    continue;
                AddLink(node, node.x - 1, node.y - 1, 1.4f);
                AddLink(node, node.x - 1, node.y + 1, 1.4f);
                AddLink(node, node.x + 1, node.y - 1, 1.4f);
                AddLink(node, node.x + 1, node.y + 1, 1.4f);
                AddLink(node, node.x - 1, node.y, 1f);
                AddLink(node, node.x + 1, node.y, 1f);
                AddLink(node, node.x, node.y - 1, 1f);
                AddLink(node, node.x, node.y + 1, 1f);
            }
        }
    }

    private void AddLink(AStarNode node, int x, int y, float g)
    {
        //判断是否在地图范围内
        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
            return;

        //在范围内 判断是否为阻挡
        AStarNode neighbour = nodes[x, y];
        if (neighbour.type == E_Node_Type.Stop)
            return;

        node.links.Add(new NodeLink(neighbour,g));
    }

    public E_Node_Type GetNodeType(int x, int y)
    {
        return nodes[x, y].type;
    }

    public List<Vector2Int> FindPathPositions(Vector2Int start, Vector2Int end)
    {
        List<AStarNode> path = FindPath(start, end);
        if (path == null)
            return null;
        List<Vector2Int> positions = new List<Vector2Int>(path.Count);
        foreach (AStarNode node in path)
        {
            positions.Add(new Vector2Int(node.x, node.y));
        }
        return positions;
    }
}
