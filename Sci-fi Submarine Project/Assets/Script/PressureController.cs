using UnityEngine;
using UnityEngine.InputSystem;

public class PressureNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Keyboard Settings")]
    [SerializeField] private Key activationKey = Key.E;

    // UI Hold State
    private bool isDepressurizingHeld = false;

    void Update()
    {
        // Only active at Pressure node
        if (nodeManager.currentNode != NodeType.Pressure)
            return;

        HandleKeyboardInput();
        HandleUIInput();
    }

    // =========================
    // KEYBOARD INPUT (UNCHANGED)
    // =========================
    private void HandleKeyboardInput()
    {
        if (Keyboard.current[activationKey].isPressed)
        {
            systemManager.ReducePressure(Time.deltaTime);
        }
    }

    // =========================
    // UI INPUT (NEW)
    // =========================
    private void HandleUIInput()
    {
        if (isDepressurizingHeld)
        {
            systemManager.ReducePressure(Time.deltaTime);
        }
    }

    // =========================
    // BUTTON EVENTS (FOR UI)
    // =========================
    public void StartDepressurizing()
    {
        isDepressurizingHeld = true;
    }

    public void StopDepressurizing()
    {
        isDepressurizingHeld = false;
    }
}