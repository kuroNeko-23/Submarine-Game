using UnityEngine;

public enum NodeType
{
    ControlPanel,
    Cooling,
    Reactor,
    Pressure
}

public class NodeManager : MonoBehaviour
{
    public NodeType currentNode;

    public void SetNode(NodeType node)
    {
        currentNode = node;
    }
}