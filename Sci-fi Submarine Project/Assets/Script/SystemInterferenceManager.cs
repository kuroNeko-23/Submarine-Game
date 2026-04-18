using UnityEngine;
using Sirenix.OdinInspector;

public class SystemInterferenceManager : MonoBehaviour
{
    public enum SystemType
    {
        None,
        Power,
        Heat,
        Pressure
    }

    [Header("Panels")]
    [SerializeField] private GameObject powerPanel;
    [SerializeField] private GameObject heatPanel;
    [SerializeField] private GameObject pressurePanel;

    [Header("Error UI")]
    [SerializeField] private GameObject powerErrorUI;
    [SerializeField] private GameObject heatErrorUI;
    [SerializeField] private GameObject pressureErrorUI;

    [Header("State")]
    [ReadOnly] public SystemType currentMalfunction = SystemType.None;

    // =========================
    // DEBUG BUTTONS
    // =========================

    [Button("Trigger Random Malfunction")]
    public void DebugTriggerRandomMalfunction()
    {
        int rand = Random.Range(1, 4);
        TriggerMalfunction((SystemType)rand);
    }

    [Button("Break Power")]
    public void DebugBreakPower() => TriggerMalfunction(SystemType.Power);

    [Button("Break Heat")]
    public void DebugBreakHeat() => TriggerMalfunction(SystemType.Heat);

    [Button("Break Pressure")]
    public void DebugBreakPressure() => TriggerMalfunction(SystemType.Pressure);

    [Button("Reset Malfunctions")]
    public void DebugReset()
    {
        ResetAll();
    }

    // =========================
    // CORE LOGIC
    // =========================

    public void TriggerMalfunction(SystemType type)
    {
        ResetAll();

        currentMalfunction = type;

        switch (type)
        {
            case SystemType.Power:
                powerErrorUI.SetActive(true);
                break;

            case SystemType.Heat:
                heatErrorUI.SetActive(true);
                break;

            case SystemType.Pressure:
                pressureErrorUI.SetActive(true);
                break;
        }

        Debug.Log($"⚠ {type} system malfunction!");
    }

    private void ResetAll()
    {
        currentMalfunction = SystemType.None;

        powerErrorUI.SetActive(false);
        heatErrorUI.SetActive(false);
        pressureErrorUI.SetActive(false);
    }
    // ADD THIS FUNCTION
    public void ResolveMalfunction()
    {
        if (currentMalfunction == SystemType.None)
            return;

        Debug.Log($"✅ {currentMalfunction} system repaired!");

        ResetAll(); // this already hides all error UI
    }
}