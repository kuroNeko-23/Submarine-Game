using UnityEngine;
using Sirenix.OdinInspector;

public class SystemManager : MonoBehaviour
{
    // =========================
    // SYSTEM VALUES (VISIBLE)
    // =========================

    [Title("SYSTEM VALUES")]

    [ReadOnly, ProgressBar(0, 100), GUIColor(0.3f, 0.6f, 1f)]
    public float pressure = 20f;

    [ReadOnly, ProgressBar(0, 100), GUIColor(1f, 0.8f, 0.2f)]
    public float power = 50f;

    [ReadOnly, ProgressBar(0, 100), GUIColor(1f, 0.4f, 0.2f)]
    public float heat = 20f;

    [ReadOnly, ProgressBar(0, 100), GUIColor(0.2f, 1f, 0.3f)]
    public float integrity = 100f;

    // =========================
    // STATE
    // =========================

    [Title("SYSTEM STATE")]
    public bool isLocked = false;

    private bool isFailing = false;

    // 🔊 AUDIO STATE
    private bool isHeatAlarmPlaying = false;
    private bool isPressureAlarmPlaying = false;

    // =========================
    // TUNING
    // =========================

    [Title("PASSIVE SYSTEM")]
    public float passivePowerDrain = 5f;
    public float passivePressureIncrease = 2f;
    public float heatLossInDepth = 5f;

    [Title("REACTOR")]
    public float powerGenerateRate = 30f;
    public float heatFromPower = 0.5f;
    public float pressureFromPower = 0.2f;

    [Title("HEAT SYSTEM")]
    public float minSafeHeat = 20f;
    public float maxSafeHeat = 80f;
    public float heatingRate = 25f;
    public float coolingRate = 30f;

    public float heatingPowerCost = 15f;
    public float coolingPowerCost = 20f;

    [Title("HEAT ZONES")]
    public float underheatThreshold = 20f;
    public float overheatThreshold = 80f;

    [Title("PRESSURE")]
    public float depressurizeRate = 25f;
    public float pressurePowerCost = 15f;

    [Title("INTEGRITY SYSTEM")]
    public float spikeDrainRate = 5f;
    public float failureDrainRate = 15f;

    public float resolveDuration = 2.5f;
    public float resolveHeatReduction = 30f;
    public float resolvePressureReduction = 30f;

    private float resolveTimer = 0f;

    // =========================
    // DEBUG
    // =========================

    [Title("DEBUG CONTROLS")]

    [Button("Add Heat (+20)")]
    private void DebugAddHeat() => heat += 20f;

    [Button("Reduce Heat (-20)")]
    private void DebugReduceHeat() => heat -= 20f;

    [Button("Add Pressure (+20)")]
    private void DebugAddPressure() => pressure += 20f;

    [Button("Reduce Pressure (-20)")]
    private void DebugReducePressure() => pressure -= 20f;

    [Button("Add Power (+30)")]
    private void DebugAddPower() => power += 30f;

    [Button("Drain Power (-30)")]
    private void DebugDrainPower() => power -= 30f;

    [Button("Trigger Heat Spike")]
    private void DebugHeatSpike() => heat = 100f;

    [Button("Trigger Pressure Spike")]
    private void DebugPressureSpike() => pressure = 100f;

    [Button("Trigger Full Failure")]
    private void DebugFullFailure()
    {
        heat = 100f;
        pressure = 100f;
    }

    [Button("Reset All Systems")]
    private void DebugResetAll()
    {
        heat = 20f;
        pressure = 20f;
        power = 50f;
        integrity = 100f;

        isLocked = false;
        isFailing = false;
        resolveTimer = 0f;
    }

    // =========================
    // UPDATE LOOP
    // =========================

    void Update()
    {
        PassiveSimulation();

        CheckFailureState();
        HandleIntegrity();

        HandleOverloadAudio(); // 🔥 ADD THIS HERE

        ClampValues();
    }
    // =========================
    // PASSIVE SYSTEM
    // =========================

    private void PassiveSimulation()
    {
        power -= passivePowerDrain * Time.deltaTime;
        pressure += passivePressureIncrease * Time.deltaTime;
        heat -= heatLossInDepth * Time.deltaTime;
    }

    // =========================
    // REACTOR
    // =========================

    public void GeneratePower(float deltaTime)
    {
        if (isLocked) return;

        power += powerGenerateRate * deltaTime;
        heat += heatFromPower * power * deltaTime;
    }

    // =========================
    // HEAT SYSTEM
    // =========================

    private bool IsUnderheated() => heat <= underheatThreshold;
    private bool IsOverheated() => heat >= overheatThreshold;

    private bool IsHeatCritical()
    {
        return IsUnderheated() || IsOverheated();
    }

    public void HeatUp(float deltaTime)
    {
        if (isLocked || power <= 0f) return;

        heat += heatingRate * deltaTime;
        power -= heatingPowerCost * deltaTime;
    }

    public void ApplyCooling(float deltaTime)
    {
        if (isLocked || power <= 0f) return;

        heat -= coolingRate * deltaTime;
        power -= coolingPowerCost * deltaTime;
    }

    // =========================
    // PRESSURE SYSTEM
    // =========================

    public void ReducePressure(float deltaTime)
    {
        if (isLocked || power <= 0f) return;

        pressure -= depressurizeRate * deltaTime;
        power -= pressurePowerCost * deltaTime;
    }

    // =========================
    // OVERLOAD AUDIO SYSTEM
    // =========================

    private void HandleOverloadAudio()
{
    bool heatCritical = heat <= underheatThreshold || heat >= overheatThreshold;
    bool pressureCritical = pressure >= 100f;

    // =========================
    // HEAT AUDIO
    // =========================
    if (heatCritical)
    {
        if (!isHeatAlarmPlaying)
        {
            isHeatAlarmPlaying = true;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayHeatOverloadAlarm();
        }
    }
    else
    {
        if (isHeatAlarmPlaying)
        {
            isHeatAlarmPlaying = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopHeatOverloadAlarm();
        }
    }

    // =========================
    // PRESSURE AUDIO
    // =========================
    if (pressureCritical)
    {
        if (!isPressureAlarmPlaying)
        {
            isPressureAlarmPlaying = true;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayPressureOverloadAlarm();
        }
    }
    else
    {
        if (isPressureAlarmPlaying)
        {
            isPressureAlarmPlaying = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopPressureOverloadAlarm();
        }
    }
}

    // =========================
    // INTEGRITY SYSTEM
    // =========================

    private void CheckFailureState()
    {
        bool pressureMax = pressure >= 100f;
        bool heatCritical = IsHeatCritical();

        if (pressureMax && heatCritical)
        {
            isLocked = true;
            isFailing = true;
        }
    }

    private void HandleIntegrity()
    {
        bool pressureSpike = pressure >= 100f;
        bool heatCritical = IsHeatCritical();

        if (isFailing)
        {
            integrity -= failureDrainRate * Time.deltaTime;
            return;
        }

        if (pressureSpike || heatCritical)
        {
            integrity -= spikeDrainRate * Time.deltaTime;
        }
    }

    public void ResolveFailure(float deltaTime)
    {
        if (!isLocked || power <= 0f) return;

        resolveTimer += deltaTime;

        if (resolveTimer >= resolveDuration)
        {
            resolveTimer = 0f;

            isLocked = false;
            isFailing = false;

            heat -= resolveHeatReduction;
            pressure -= resolvePressureReduction;
        }
    }

    public void ResetResolveProgress()
    {
        resolveTimer = 0f;
    }

    // =========================
    // CLAMP
    // =========================

    private void ClampValues()
    {
        pressure = Mathf.Clamp(pressure, 0, 100);
        power = Mathf.Clamp(power, 0, 100);
        heat = Mathf.Clamp(heat, 0, 100);
        integrity = Mathf.Clamp(integrity, 0, 100);
    }
}