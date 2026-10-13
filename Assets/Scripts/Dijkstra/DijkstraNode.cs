using System.Collections.Generic;

public struct DijkstraNodeLink
{
    public DijkstraNode node;
    public float cost;

    public DijkstraNodeLink(DijkstraNode node, float cost)
    {
        this.node = node;
        this.cost = cost;
    }
}

public class DijkstraNode
{
    public int x;
    public int y;

    //从起点沿当前已知最佳路径到达此节点的累计代价
    public float g;
    public DijkstraNode father;

    public E_Node_Type type;
    public E_Search_State state;
    public List<DijkstraNodeLink> links = new List<DijkstraNodeLink>(8);

    public DijkstraNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        state = E_Search_State.None;
    }
}
