using UnityEngine;
using UnityEngine.InputSystem;

public class ControlPanelNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Settings")]
    [SerializeField] private Key repairKey = Key.R;

    void Update()
    {
        if (nodeManager.currentNode != NodeType.ControlPanel)
            return;

        if (!systemManager.isLocked)
            return;

        if (Keyboard.current[repairKey].isPressed)
        {
            systemManager.ResolveFailure();
        }
        else
        {
            // Reset progress if player releases key
            // (important for “hold to repair” feel)
            //systemManager.ResetResolveProgress();
        }
    }
    
}