using UnityEngine;
using UnityEngine.InputSystem;

public class ReactorNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Keyboard Settings")]
    [SerializeField] private Key activationKey = Key.W;

    // UI Hold State
    private bool isGeneratingHeld = false;

    void Update()
    {
        // Only allow interaction at Reactor node
        if (nodeManager.currentNode != NodeType.Reactor)
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
            systemManager.GeneratePower(Time.deltaTime);
        }
    }

    // =========================
    // UI INPUT (NEW)
    // =========================
    private void HandleUIInput()
    {
        if (isGeneratingHeld)
        {
            systemManager.GeneratePower(Time.deltaTime);
        }
    }

    // =========================
    // BUTTON EVENTS (FOR UI)
    // =========================
    public void StartGenerating()
    {
        isGeneratingHeld = true;
    }

    public void StopGenerating()
    {
        isGeneratingHeld = false;
    }
}