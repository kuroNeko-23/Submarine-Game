using UnityEngine;
using UnityEngine.InputSystem;

public class CoolingNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Settings")]
    [SerializeField] private Key coolKey = Key.C;
    [SerializeField] private Key heatKey = Key.V;

    void Update()
    {
        // Only active at Cooling node
        if (nodeManager.currentNode != NodeType.Cooling)
            return;

        HandleInput();
    }

    private void HandleInput()
    {
        // Cooling
        if (Keyboard.current[coolKey].isPressed)
        {
            systemManager.ApplyCooling(Time.deltaTime);
        }

        // Heating
        if (Keyboard.current[heatKey].isPressed)
        {
            systemManager.HeatUp(Time.deltaTime);
        }
    }
}