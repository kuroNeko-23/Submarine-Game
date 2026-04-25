using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

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

    [Header("Electricity VFX")]
    [SerializeField] private ParticleSystem electricityVFX;
    [Header("Hold To Fix Text")]
    [SerializeField] private GameObject electricityHoldToFixText;

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
    void Awake()
    {
        electricityHoldToFixText.SetActive(false);
    }
    public void TriggerMalfunction(SystemType type)
    {
        ResetAll();

        currentMalfunction = type;

        // 🔥 VFX
        if (electricityVFX != null)
            electricityVFX.Play();
        if(electricityHoldToFixText != null)
            electricityHoldToFixText.SetActive(true);
        // 🔊 AUDIO (ADD THIS)
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayElectricity();
        // 🔊 UI SFX
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayErrorPanelSFX();

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

        // 🔥 VFX
        if (electricityVFX != null)
            electricityVFX.Stop();

        if(electricityHoldToFixText != null)
            electricityHoldToFixText.SetActive(false);

        // 🔊 AUDIO (ADD THIS)
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopElectricity();
    }

    public void ResolveMalfunction()
    {
        if (currentMalfunction == SystemType.None)
            return;

        Debug.Log($"✅ {currentMalfunction} system repaired!");
        if(electricityHoldToFixText != null)
            electricityHoldToFixText.SetActive(false);
        // 🔊 UI SFX
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayErrorPanelSFX();

        ResetAll();
    }
    // =========================
    // RANDOM TRIGGER (NEW)
    // =========================

    public void TriggerRandomInterference()
    {
        List<SystemType> list = new List<SystemType>
        {
            SystemType.Power,
            SystemType.Heat,
            SystemType.Pressure
        };

        SystemType selected = list[Random.Range(0, list.Count)];
        TriggerMalfunction(selected);
    }
}