using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class LeakMalfunctionManager : MonoBehaviour
{
    public enum LeakType
    {
        None,
        ReactorLeak,
        PressureLeak,
        HeatPipeLeak
    }

    [Header("References")]
    [SerializeField] private SystemManager systemManager;

    [Header("State")]
    [ReadOnly] public List<LeakType> activeLeaks = new List<LeakType>();

    [Header("Leak VFX")]
    [SerializeField] private GameObject reactorLeakVFX;
    [SerializeField] private GameObject pressureLeakVFX;
    [SerializeField] private GameObject heatPipeLeakVFX;

    [Header("Hold To Fix Text")]
    [SerializeField] private GameObject reactorHoldToFixText;
    [SerializeField] private GameObject pressureHoldToFixText;
    [SerializeField] private GameObject heatPipeHoldToFixText;

    // =========================
    // TUNING
    // =========================

    [Header("Reactor Leak")]
    public float reactorHeatRate = 40f;
    public float reactorPowerDrain = 25f;

    [Header("Pressure Leak")]
    public float pressureIncreaseRate = 40f;

    [Header("Heat Pipe Leak")]
    public float heatDropRate = 50f;

    // =========================
    // INIT
    // =========================
    void Awake()
    {
        reactorHoldToFixText.SetActive(false);
        pressureHoldToFixText.SetActive(false);
        heatPipeHoldToFixText.SetActive(false);
    }

    void Start()
    {
        ResetAllVFX();
    }
    

    void Update()
    {
        ApplyAllLeaks();
    }

    private void ApplyAllLeaks()
    {
        foreach (var leak in activeLeaks)
        {
            switch (leak)
            {
                case LeakType.ReactorLeak:
                    ApplyReactorLeak();
                    break;

                case LeakType.PressureLeak:
                    ApplyPressureLeak();
                    break;

                case LeakType.HeatPipeLeak:
                    ApplyHeatPipeLeak();
                    break;
            }
        }
    }

    // =========================
    // EFFECTS
    // =========================

    private void ApplyReactorLeak()
    {
        systemManager.heat += reactorHeatRate * Time.deltaTime;
        systemManager.power -= reactorPowerDrain * Time.deltaTime;
    }

    private void ApplyPressureLeak()
    {
        systemManager.pressure += pressureIncreaseRate * Time.deltaTime;
    }

    private void ApplyHeatPipeLeak()
    {
        systemManager.heat -= heatDropRate * Time.deltaTime;
    }

    // =========================
    // TRIGGER
    // =========================

    public void TriggerLeak(LeakType type)
    {
        if (type == LeakType.None) return;

        if (!activeLeaks.Contains(type))
        {
            activeLeaks.Add(type);
            EnableLeakVFX(type);
            EnableHoldToFixText(type);

            // 🔊 AUDIO (ADD THIS)
            PlayLeakAudio(type);

            Debug.Log($"⚠ {type} started");
        }
    }

    // =========================
    // RESOLVE
    // =========================

    public void ResolveLeak(LeakType type)
    {
        if (!activeLeaks.Contains(type))
        {
            Debug.Log("Wrong leak fix!");
            return;
        }

        activeLeaks.Remove(type);
        DisableLeakVFX(type);
        DisableHoldToFixText(type);
        // 🔊 AUDIO (ADD THIS)
        StopLeakAudio(type);

        Debug.Log($"✅ {type} fixed");
    }

    // =========================
    // VFX CONTROL
    // =========================

    private void EnableLeakVFX(LeakType type)
    {
        switch (type)
        {
            case LeakType.ReactorLeak:
                reactorLeakVFX.SetActive(true);
                break;

            case LeakType.PressureLeak:
                pressureLeakVFX.SetActive(true);
                break;

            case LeakType.HeatPipeLeak:
                heatPipeLeakVFX.SetActive(true);
                break;
        }
    }

    private void DisableLeakVFX(LeakType type)
    {
        switch (type)
        {
            case LeakType.ReactorLeak:
                reactorLeakVFX.SetActive(false);
                break;

            case LeakType.PressureLeak:
                pressureLeakVFX.SetActive(false);
                break;

            case LeakType.HeatPipeLeak:
                heatPipeLeakVFX.SetActive(false);
                break;
        }
    }

    private void ResetAllVFX()
    {
        reactorLeakVFX.SetActive(false);
        pressureLeakVFX.SetActive(false);
        heatPipeLeakVFX.SetActive(false);
    }

    // =========================
    // AUDIO CONTROL (NEW)
    // =========================

    private void PlayLeakAudio(LeakType type)
    {
        if (AudioManager.Instance == null) return;

        switch (type)
        {
            case LeakType.ReactorLeak:
                AudioManager.Instance.PlayReactorLeak();
                break;

            case LeakType.PressureLeak:
                AudioManager.Instance.PlayPressureLeak();
                break;

            case LeakType.HeatPipeLeak:
                AudioManager.Instance.PlayHeatLeak();
                break;
        }
    }

    private void StopLeakAudio(LeakType type)
    {
        if (AudioManager.Instance == null) return;

        switch (type)
        {
            case LeakType.ReactorLeak:
                AudioManager.Instance.StopReactorLeak();
                break;

            case LeakType.PressureLeak:
                AudioManager.Instance.StopPressureLeak();
                break;

            case LeakType.HeatPipeLeak:
                AudioManager.Instance.StopHeatLeak();
                break;
        }
    }

    // =========================
    // DEBUG
    // =========================

    [Button("Reactor Leak")]
    private void DebugReactor() => TriggerLeak(LeakType.ReactorLeak);

    [Button("Pressure Leak")]
    private void DebugPressure() => TriggerLeak(LeakType.PressureLeak);

    [Button("Heat Pipe Leak")]
    private void DebugHeatPipe() => TriggerLeak(LeakType.HeatPipeLeak);

    [Button("Clear Leak")]
    private void DebugClear()
    {
        foreach (var leak in new List<LeakType>(activeLeaks))
        {
            ResolveLeak(leak);
        }
    }

    // =========================
    void EnableHoldToFixText(LeakType type)
    {
        switch (type)
        {
            case LeakType.ReactorLeak:
                reactorHoldToFixText.SetActive(true);
                break;

            case LeakType.PressureLeak:
                pressureHoldToFixText.SetActive(true);
                break;

            case LeakType.HeatPipeLeak:
                heatPipeHoldToFixText.SetActive(true);
                break;
        }
    }
    void DisableHoldToFixText(LeakType type)
    {
        switch (type)
        {
            case LeakType.ReactorLeak:
                reactorHoldToFixText.SetActive(false);
                break;

            case LeakType.PressureLeak:
                pressureHoldToFixText.SetActive(false);
                break;

            case LeakType.HeatPipeLeak:
                heatPipeHoldToFixText.SetActive(false);
                break;
        }
    }
}