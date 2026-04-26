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
    public float heat = 50f;

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
    //public float spikeDrainRate = 5f; // (keep if you want fallback)

    public float pressureSpikeDrainRate = 8f;   // 🔴 more dangerous
    public float heatSpikeDrainRate = 3f;       // 🟠 slower damage

    public float failureDrainRate = 15f;

    // 🔥 NEW: passive regen rate
    public float integrityRegenRate = 0.1f;

    public float resolveDuration = 2.5f;
    public float resolveHeatReduction = 30f;
    public float resolvePressureReduction = 30f;

    [Header("Emergency Lights")]
    [SerializeField] private LightFlicker emergencyLights;

    // =========================
    // UPDATE LOOP
    // =========================

    void Update()
    {
        PassiveSimulation();

        CheckFailureState();
        HandleIntegrity();

        HandleIntegrityRegen(); // 🔥 ADDED

        LowPowerAudio();
        HandleOverloadAudio();

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

    public void LowPowerAudio()
    {
        if (power <= 15f)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLowPowerAlarm();
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopLowPowerAlarm();
        }
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
        bool heatCritical = IsHeatCritical();
        bool pressureCritical = pressure >= 100f;

        if (heatCritical)
        {
            if (!isHeatAlarmPlaying)
            {
                isHeatAlarmPlaying = true;
                AudioManager.Instance?.PlayHeatOverloadAlarm();
            }
        }
        else
        {
            if (isHeatAlarmPlaying)
            {
                isHeatAlarmPlaying = false;
                AudioManager.Instance?.StopHeatOverloadAlarm();
            }
        }

        if (pressureCritical)
        {
            if (!isPressureAlarmPlaying)
            {
                isPressureAlarmPlaying = true;
                AudioManager.Instance?.PlayPressureOverloadAlarm();
            }
        }
        else
        {
            if (isPressureAlarmPlaying)
            {
                isPressureAlarmPlaying = false;
                AudioManager.Instance?.StopPressureOverloadAlarm();
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

        if (!isLocked && pressureMax && heatCritical)
        {
            isLocked = true;
            isFailing = true;

            Debug.Log("🚨 SYSTEM FAILURE TRIGGERED");

            AudioManager.Instance?.PlaySystemFailureAlarm();

            if (emergencyLights != null)
                emergencyLights.StartFlicker();
        }
    }

    private void HandleIntegrity()
    {
        bool pressureSpike = pressure >= 100f;
        bool heatCritical = IsHeatCritical();

        // 🔴 HARD FAILURE
        if (isFailing)
        {
            integrity -= failureDrainRate * Time.deltaTime;
            return;
        }

        // =========================
        // 🔥 SEPARATE DAMAGE LOGIC
        // =========================

        float damage = 0f;

        if (pressureSpike)
            damage += pressureSpikeDrainRate;

        if (heatCritical)
            damage += heatSpikeDrainRate;

        integrity -= damage * Time.deltaTime;
    }

    // =========================
    // 🔥 NEW: INTEGRITY REGEN
    // =========================

    private void HandleIntegrityRegen()
    {
        bool stableSystem =
            !isFailing &&
            pressure < 100f &&
            !IsHeatCritical();

        if (stableSystem)
        {
            integrity += integrityRegenRate * Time.deltaTime;
        }
    }

    // =========================
    // RESOLVE FAILURE
    // =========================

    public void ResolveFailure()
    {
        if (!isLocked) return;

        isLocked = false;
        isFailing = false;

        // +10 INTEGRITY BOOST
        integrity += 10f;

        pressure = Mathf.Clamp(pressure - resolvePressureReduction, 0f, 100f);

        if (heat >= overheatThreshold)
            heat = maxSafeHeat;
        else if (heat <= underheatThreshold)
            heat = minSafeHeat;

        Debug.Log("✅ SYSTEM RECOVERED (+10 Integrity)");

        AudioManager.Instance?.StopSystemFailureAlarm();

        if (emergencyLights != null)
            emergencyLights.StopFlicker();
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
    [Title("DEBUG - QUICK TEST STATES")]

    // =========================
    // 🔴 PRESSURE SPIKE
    // =========================
    [Button("⚠️ Trigger Pressure Spike")]
    private void DebugPressureCritical()
    {
        pressure = 100f;
        Debug.Log("DEBUG: Pressure set to CRITICAL");
    }

    // =========================
    // 🔥 HEAT OVERLOAD
    // =========================
    [Button("🔥 Trigger Heat Overload")]
    private void DebugHeatCriticalHigh()
    {
        heat = 100f;
        Debug.Log("DEBUG: Heat set to OVERHEAT");
    }

    // =========================
    // ❄️ HEAT UNDERLOAD
    // =========================
    [Button("❄️ Trigger Underheat")]
    private void DebugHeatCriticalLow()
    {
        heat = 0f;
        Debug.Log("DEBUG: Heat set to UNDERHEAT");
    }

    // =========================
    // 💥 DOUBLE CRITICAL (STACK TEST)
    // =========================
    [Button("💥 Trigger FULL CRITICAL (Heat + Pressure)")]
    private void DebugFullCritical()
    {
        pressure = 100f;
        heat = 100f;
        Debug.Log("DEBUG: FULL CRITICAL STATE");
    }

    // =========================
    // 🔋 LOW POWER
    // =========================
    [Button("🔋 Trigger Low Power")]
    private void DebugLowPower()
    {
        power = 5f;
        Debug.Log("DEBUG: Low Power Triggered");
    }

    // =========================
    // 🧯 STABILIZE SYSTEM (FOR REGEN TEST)
    // =========================
    [Button("🧯 Stabilize Systems (Test Regen)")]
    private void DebugStableSystem()
    {
        pressure = 20f;
        heat = 50f;
        power = 50f;
        isLocked = false;
        isFailing = false;

        Debug.Log("DEBUG: System Stabilized (Regen should start)");
    }

    // =========================
    // ❤️ DAMAGE INTEGRITY
    // =========================
    [Button("💔 Damage Integrity (-25)")]
    private void DebugDamageIntegrity()
    {
        integrity -= 25f;
        Debug.Log("DEBUG: Integrity Damaged");
    }

    // =========================
    // 💚 HEAL INTEGRITY
    // =========================
    [Button("💚 Heal Integrity (+25)")]
    private void DebugHealIntegrity()
    {
        integrity += 25f;
        Debug.Log("DEBUG: Integrity Healed");
    }
}