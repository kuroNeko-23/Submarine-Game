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

    [System.Serializable]
    public class LeakInstance
    {
        public LeakType type;
        public float timer;
        public float activationTime;
        public bool isActive;

        public LeakInstance(LeakType type, float activationTime)
        {
            this.type = type;
            this.activationTime = activationTime;
            this.timer = 0f;
            this.isActive = false;
        }
    }

    [Header("References")]
    [SerializeField] private SystemManager systemManager;

    [Header("State")]
    [ReadOnly] public List<LeakInstance> activeLeaks = new List<LeakInstance>();

    [Header("Leak Delay System")]
    [SerializeField] private float leakWarningMin = 4f;
    [SerializeField] private float leakWarningMax = 7f;

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
        UpdateLeakStates();
        ApplyAllLeaks();
    }

    // =========================
    // CORE LOOP
    // =========================

    private void UpdateLeakStates()
    {
        foreach (var leak in activeLeaks)
        {
            if (leak.isActive) continue;

            leak.timer += Time.deltaTime;

            if (leak.timer >= leak.activationTime)
            {
                leak.isActive = true;
                Debug.Log($"🔥 {leak.type} is now ACTIVE");
            }
        }
    }

    private void ApplyAllLeaks()
    {
        if (systemManager == null) return;

        foreach (var leak in activeLeaks)
        {
            if (!leak.isActive) continue;

            switch (leak.type)
            {
                case LeakType.ReactorLeak:
                    systemManager.heat += reactorHeatRate * Time.deltaTime;
                    systemManager.power -= reactorPowerDrain * Time.deltaTime;
                    break;

                case LeakType.PressureLeak:
                    systemManager.pressure += pressureIncreaseRate * Time.deltaTime;
                    break;

                case LeakType.HeatPipeLeak:
                    systemManager.heat -= heatDropRate * Time.deltaTime;
                    break;
            }
        }
    }

    // =========================
    // TRIGGER
    // =========================

    public void TriggerLeak(LeakType type)
    {
        if (type == LeakType.None) return;

        // prevent duplicate
        foreach (var leak in activeLeaks)
        {
            if (leak.type == type)
                return;
        }

        float delay = Random.Range(leakWarningMin, leakWarningMax);
        LeakInstance newLeak = new LeakInstance(type, delay);

        activeLeaks.Add(newLeak);

        EnableLeakVFX(type);
        EnableHoldToFixText(type);
        PlayLeakAudio(type);

        Debug.Log($"⚠ {type} started (WARNING)");
    }
    public bool HasLeak(LeakType type)
    {
        return activeLeaks.Exists(l => l.type == type);
    }

    // =========================
    // RESOLVE
    // =========================

    public void ResolveLeak(LeakType type)
    {
        LeakInstance target = activeLeaks.Find(l => l.type == type);

        if (target == null)
        {
            Debug.Log("Wrong leak fix!");
            return;
        }

        activeLeaks.Remove(target);

        DisableLeakVFX(type);
        DisableHoldToFixText(type);
        StopLeakAudio(type);

        Debug.Log($"✅ {type} fixed");
    }

    // =========================
    // RANDOM
    // =========================

    public void TriggerRandomLeak()
    {
        List<LeakType> available = GetAvailableLeaks();

        if (available.Count == 0) return;

        TriggerLeak(available[Random.Range(0, available.Count)]);
    }

    public void TriggerMultipleLeaks(int count)
    {
        List<LeakType> available = GetAvailableLeaks();
        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, available.Count);
            TriggerLeak(available[index]);
            available.RemoveAt(index);
        }
    }

    private List<LeakType> GetAvailableLeaks()
    {
        List<LeakType> all = new List<LeakType>
        {
            LeakType.ReactorLeak,
            LeakType.PressureLeak,
            LeakType.HeatPipeLeak
        };

        foreach (var leak in activeLeaks)
        {
            all.Remove(leak.type);
        }

        return all;
    }

    // =========================
    // VFX
    // =========================

    private void EnableLeakVFX(LeakType type)
    {
        if (type == LeakType.ReactorLeak) reactorLeakVFX.SetActive(true);
        if (type == LeakType.PressureLeak) pressureLeakVFX.SetActive(true);
        if (type == LeakType.HeatPipeLeak) heatPipeLeakVFX.SetActive(true);
    }

    private void DisableLeakVFX(LeakType type)
    {
        if (type == LeakType.ReactorLeak) reactorLeakVFX.SetActive(false);
        if (type == LeakType.PressureLeak) pressureLeakVFX.SetActive(false);
        if (type == LeakType.HeatPipeLeak) heatPipeLeakVFX.SetActive(false);
    }

    private void ResetAllVFX()
    {
        reactorLeakVFX.SetActive(false);
        pressureLeakVFX.SetActive(false);
        heatPipeLeakVFX.SetActive(false);
    }

    // =========================
    // AUDIO
    // =========================

    private void PlayLeakAudio(LeakType type)
    {
        if (AudioManager.Instance == null) return;

        if (type == LeakType.ReactorLeak) AudioManager.Instance.PlayReactorLeak();
        if (type == LeakType.PressureLeak) AudioManager.Instance.PlayPressureLeak();
        if (type == LeakType.HeatPipeLeak) AudioManager.Instance.PlayHeatLeak();
    }

    private void StopLeakAudio(LeakType type)
    {
        if (AudioManager.Instance == null) return;

        if (type == LeakType.ReactorLeak) AudioManager.Instance.StopReactorLeak();
        if (type == LeakType.PressureLeak) AudioManager.Instance.StopPressureLeak();
        if (type == LeakType.HeatPipeLeak) AudioManager.Instance.StopHeatLeak();
    }

    // =========================
    // UI
    // =========================

    void EnableHoldToFixText(LeakType type)
    {
        if (type == LeakType.ReactorLeak) reactorHoldToFixText.SetActive(true);
        if (type == LeakType.PressureLeak) pressureHoldToFixText.SetActive(true);
        if (type == LeakType.HeatPipeLeak) heatPipeHoldToFixText.SetActive(true);
    }

    void DisableHoldToFixText(LeakType type)
    {
        if (type == LeakType.ReactorLeak) reactorHoldToFixText.SetActive(false);
        if (type == LeakType.PressureLeak) pressureHoldToFixText.SetActive(false);
        if (type == LeakType.HeatPipeLeak) heatPipeHoldToFixText.SetActive(false);
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

    [Button("Clear All Leaks")]
    private void DebugClear()
    {
        foreach (var leak in new List<LeakInstance>(activeLeaks))
        {
            ResolveLeak(leak.type);
        }
    }
}