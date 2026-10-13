using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSNode
{
    //格子对象的坐标
    public int x;
    public int y;

    //父对象
    public BFSNode father;

    public E_Node_Type type;
    public E_Search_State state;

    public List<BFSNode> links = new List<BFSNode>(4);

    public BFSNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.state = E_Search_State.None;
    }
}
