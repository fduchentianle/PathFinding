using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFSNode
{
    //格子对象的坐标
    public int x;
    public int y;

    //父对象
    public DFSNode father;

    public E_Node_Type type;
    public E_Search_State state;

    public List<DFSNode> links = new List<DFSNode>(4);

    public DFSNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.state = E_Search_State.None;
    }
}
