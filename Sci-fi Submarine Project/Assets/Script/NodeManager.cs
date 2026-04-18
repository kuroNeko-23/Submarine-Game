using UnityEngine;

public enum NodeType
{
    ControlPanel,
    Cooling,
    Reactor_Pipes,
    Reactor,
    Electricity,
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