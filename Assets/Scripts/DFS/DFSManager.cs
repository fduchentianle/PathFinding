using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 四方向DFS寻路管理器
/// </summary>
public class DFSManager : BaseManager<DFSManager>, IPathFinder
{
    //地图的宽高
    private int mapW;
    private int mapH;

    //地图的所有格子
    public DFSNode[,] nodes;
    //待处理节点栈（后进先出）
    private Stack<DFSNode> stack;

    private DFSManager() { }

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
        nodes = new DFSNode[w, h];
        stack = new Stack<DFSNode>();

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                DFSNode node = new DFSNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
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
    public List<DFSNode> FindPath(Vector2Int startPos, Vector2Int endPos)
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

        DFSNode start = nodes[startPos.x, startPos.y];
        DFSNode end = nodes[endPos.x, endPos.y];

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
            List<DFSNode> path = new List<DFSNode>();
            path.Add(start);
            return path;
        }

        //清空开启栈
        stack.Clear();

        //把起始格子加入开启栈
        start.father = null;
        start.state = E_Search_State.Open;
        stack.Push(start);

        while (stack.Count > 0)
        {
            DFSNode current = stack.Pop();
            current.state = E_Search_State.Close;

            if (current == end)
            {
                List<DFSNode> path = new List<DFSNode>();
                path.Add(end);
                while (end.father != null)
                {
                    path.Add(end.father);
                    end = end.father;
                }
                path.Reverse();
                return path;
            }

            foreach (DFSNode node in current.links)
            {
                if (node.state == E_Search_State.None)
                {
                    node.father = current;
                    node.state = E_Search_State.Open;
                    stack.Push(node);
                }
            }
        }

        Debug.Log("此为死路，无法找到路径");
        return null;
    }

    /// <summary>
    /// 重置地图
    /// </summary>
    public void ResetMap()
    {
        stack.Clear();

        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                nodes[i, j].type = Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk;
                nodes[i, j].state = E_Search_State.None;
            }
        }

        //预加载每个格子周围的可行走格子信息
        BuildLinks();
    }

    private void BuildLinks()
    {
        for (int i = 0; i < mapW; i++)
        {
            for (int j = 0; j < mapH; j++)
            {
                DFSNode node = nodes[i, j];
                node.links.Clear();
                if (node.type == E_Node_Type.Stop)
                    continue;
                AddLink(node, node.x - 1, node.y);
                AddLink(node, node.x + 1, node.y);
                AddLink(node, node.x, node.y - 1);
                AddLink(node, node.x, node.y + 1);
            }
        }
    }

    private void AddLink(DFSNode node, int x, int y)
    {
        //判断是否在地图范围内
        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
            return;

        //在范围内 判断是否为阻挡
        DFSNode neighbour = nodes[x, y];
        if (neighbour.type == E_Node_Type.Stop)
            return;

        node.links.Add(neighbour);
    }

    public E_Node_Type GetNodeType(int x, int y)
    {
        return nodes[x, y].type;
    }

    public List<Vector2Int> FindPathPositions(Vector2Int start, Vector2Int end)
    {
        List<DFSNode> path = FindPath(start,end);
        if (path == null)
            return null;
        List<Vector2Int> positions = new List<Vector2Int>(path.Count);
        foreach (DFSNode node in path)
        {
            positions.Add(new Vector2Int(node.x, node.y));
        }
        return positions;
    }
}
