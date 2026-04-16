using UnityEngine;
using UnityEngine.InputSystem;

public class ReactorNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Settings")]
    [SerializeField] private Key activationKey = Key.W;

    void Update()
    {
        // Only allow interaction at Reactor node
        if (nodeManager.currentNode != NodeType.Reactor)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Keyboard.current[activationKey].isPressed)
        {
            systemManager.GeneratePower(Time.deltaTime);
        }
    }
}