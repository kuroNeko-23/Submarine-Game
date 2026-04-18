using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CoolingNode : MonoBehaviour
{
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private NodeManager nodeManager;

    [Header("Keyboard Settings")]
    [SerializeField] private Key coolKey = Key.C;
    [SerializeField] private Key heatKey = Key.V;

    [Header("UI Status")]
    [SerializeField] private TMP_Text statusText;

    // UI Hold States
    private bool isCoolingHeld = false;
    private bool isHeatingHeld = false;

    void Update()
    {
        // Only active at Cooling node
        if (nodeManager.currentNode != NodeType.Cooling)
            return;

        HandleKeyboardInput();
        HandleUIInput();
        UpdateHeatStatus();
    }

    // =========================
    // KEYBOARD INPUT (UNCHANGED)
    // =========================
    private void HandleKeyboardInput()
    {
        if (Keyboard.current[coolKey].isPressed)
        {
            systemManager.ApplyCooling(Time.deltaTime);
        }

        if (Keyboard.current[heatKey].isPressed)
        {
            systemManager.HeatUp(Time.deltaTime);
        }
    }

    // =========================
    // UI INPUT (NEW)
    // =========================
    private void HandleUIInput()
    {
        if (isCoolingHeld)
        {
            systemManager.ApplyCooling(Time.deltaTime);
        }

        if (isHeatingHeld)
        {
            systemManager.HeatUp(Time.deltaTime);
        }
    }

    // =========================
    // BUTTON EVENTS (HOOK IN UI)
    // =========================
    public void StartCooling()
    {
        isCoolingHeld = true;
    }

    public void StopCooling()
    {
        isCoolingHeld = false;
    }

    public void StartHeating()
    {
        isHeatingHeld = true;
    }

    public void StopHeating()
    {
        isHeatingHeld = false;
    }

    // =========================
    // HEAT STATUS (NEW)
    // =========================
    private void UpdateHeatStatus()
    {
        if (statusText == null) return;

        float heat = systemManager.heat;

        if (heat <= systemManager.underheatThreshold)
        {
            statusText.text = "UNDERHEAT";
        }
        else if (heat >= systemManager.overheatThreshold)
        {
            statusText.text = "OVERHEAT";
        }
        else
        {
            statusText.text = "BALANCED";
        }
    }
}