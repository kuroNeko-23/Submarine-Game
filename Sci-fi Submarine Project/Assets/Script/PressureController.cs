using UnityEngine;
using UnityEngine.InputSystem;

public class PressureNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Settings")]
    [SerializeField] private Key activationKey = Key.E;

    void Update()
    {
        // Only active at Pressure node
        if (nodeManager.currentNode != NodeType.Pressure)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (Keyboard.current[activationKey].isPressed)
        {
            systemManager.ReducePressure(Time.deltaTime);
        }
    }
}