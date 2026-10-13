using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Node_Type
{
    //可以走的格子
    Walk,
    //不能走的格子
    Stop,
}

public enum E_Search_State
{
    //还未加入到开启列表和关闭列表
    None,
    //在开启列表中
    Open,
    //在关闭列表中
    Close,
}

public struct NodeLink
{
    public AStarNode node;
    public float cost;
    public NodeLink(AStarNode node, float cost)
    {
        this.node = node;
        this.cost = cost;
    }
}

public class AStarNode
{
    //格子对象的坐标
    public int x;
    public int y;

    //寻路消耗的总值（f=g+h）
    public float f;
    //离起点的距离（欧式距离）
    public float g;
    //离终点的距离（曼哈顿距离）
    public float h;
    //父对象
    public AStarNode father;

    public E_Node_Type type;
    public E_Search_State state;

    public List<NodeLink> links = new List<NodeLink>(8);

    public AStarNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.state = E_Search_State.None;
    }

}
