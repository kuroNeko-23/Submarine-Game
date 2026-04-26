using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemIntegrityPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private SystemInterferenceManager interferenceManager;
    [SerializeField] private LeakMalfunctionManager leakManager;

    // =========================
    // SLIDERS
    // =========================

    [Header("Sliders")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Slider heatSlider;
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Slider integritySlider;

    [Header("Slider Fill Images")]
    [SerializeField] private Image powerFill;
    [SerializeField] private Image heatFill;
    [SerializeField] private Image pressureFill;
    [SerializeField] private Image integrityFill;

    [Header("Critical Color")]
    [SerializeField] private Color criticalColor = Color.red;

    private Color powerDefaultColor;
    private Color heatDefaultColor;
    private Color pressureDefaultColor;
    private Color integrityDefaultColor;

    // =========================
    // STATUS TEXT
    // =========================

    [Header("Status Text UI")]
    [SerializeField] private TMP_Text electricityStatus;
    [SerializeField] private TMP_Text panelsStatus;
    [SerializeField] private TMP_Text reactorStatus;
    [SerializeField] private TMP_Text heatPipeStatus;
    [SerializeField] private TMP_Text pressurePipeStatus;

    [System.Serializable]
    public class BinaryStatus
    {
        public string normalText = "Connected";
        public string errorText = "Disconnected";
    }

    [Header("Status Config")]
    [SerializeField] private BinaryStatus electricityConfig;
    [SerializeField] private BinaryStatus panelsConfig;
    [SerializeField] private BinaryStatus reactorConfig;
    [SerializeField] private BinaryStatus heatPipeConfig;
    [SerializeField] private BinaryStatus pressurePipeConfig;

    // =========================
    // WARNING VFX
    // =========================

    [Header("⚠ System Warning Images || Core System")]
    [SerializeField] private Image PowerWarning;
    [SerializeField] private Image HeatWarning;
    [SerializeField] private Image PressureWarning;
    [SerializeField] private Image IntegrityWarning;

    [SerializeField] private float flickerSpeed = 8f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;

    // =========================
    // SETTINGS
    // =========================

    [Header("Slider Settings")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothSpeed = 5f;

    void Start()
    {
        InitSlider(powerSlider);
        InitSlider(heatSlider);
        InitSlider(pressureSlider);
        InitSlider(integritySlider);

        CacheDefaultColors();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (systemManager == null) return;

        UpdateSliders();
        UpdateStatuses();
        UpdateWarnings();
        UpdateCriticalColors();
    }

    // =========================
    // SLIDERS
    // =========================

    private void UpdateSliders()
    {
        UpdateSlider(powerSlider, systemManager.power);
        UpdateSlider(heatSlider, systemManager.heat);
        UpdateSlider(pressureSlider, systemManager.pressure);
        UpdateSlider(integritySlider, systemManager.integrity);
    }

    private void InitSlider(Slider slider)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.interactable = false;
    }

    private void UpdateSlider(Slider slider, float targetValue)
    {
        if (slider == null) return;

        if (useSmoothing)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
        else
        {
            slider.value = targetValue;
        }
    }

    // =========================
    // STATUS (FIXED)
    // =========================

    private void UpdateStatuses()
    {
        UpdateElectricity();
        UpdatePanels();
        UpdateReactor();
        UpdateHeatPipe();
        UpdatePressurePipe();
    }

    private void UpdateElectricity()
    {
        if (electricityStatus == null || interferenceManager == null) return;

        bool isBroken = interferenceManager.currentMalfunction != SystemInterferenceManager.SystemType.None;

        electricityStatus.text = isBroken
            ? electricityConfig.errorText
            : electricityConfig.normalText;
    }

    private void UpdatePanels()
    {
        if (panelsStatus == null || interferenceManager == null) return;

        bool isBroken = interferenceManager.currentMalfunction != SystemInterferenceManager.SystemType.None;

        panelsStatus.text = isBroken
            ? panelsConfig.errorText
            : panelsConfig.normalText;
    }

    private void UpdateReactor()
    {
        if (reactorStatus == null || leakManager == null) return;

        bool isBroken = leakManager.HasLeak(LeakMalfunctionManager.LeakType.ReactorLeak); // ✅ FIXED

        reactorStatus.text = isBroken
            ? reactorConfig.errorText
            : reactorConfig.normalText;
    }

    private void UpdateHeatPipe()
    {
        if (heatPipeStatus == null || leakManager == null) return;

        bool isBroken = leakManager.HasLeak(LeakMalfunctionManager.LeakType.HeatPipeLeak); // ✅ FIXED

        heatPipeStatus.text = isBroken
            ? heatPipeConfig.errorText
            : heatPipeConfig.normalText;
    }

    private void UpdatePressurePipe()
    {
        if (pressurePipeStatus == null || leakManager == null) return;

        bool isBroken = leakManager.HasLeak(LeakMalfunctionManager.LeakType.PressureLeak); // ✅ FIXED

        pressurePipeStatus.text = isBroken
            ? pressurePipeConfig.errorText
            : pressurePipeConfig.normalText;
    }

    // =========================
    // WARNING
    // =========================

    private void UpdateWarnings()
    {
        HandlePowerWarning();
        HandleHeatWarning();
        HandlePressureWarning();
        HandleIntegrityWarning();
    }

    private void HandlePowerWarning()
    {
        bool warning = systemManager.power <= 20f;
        if (warning) Flicker(PowerWarning);
        else SetInvisible(PowerWarning);
    }

    private void HandleHeatWarning()
    {
        float heat = systemManager.heat;

        bool warning = heat <= systemManager.underheatThreshold ||
                       heat >= systemManager.overheatThreshold;

        if (warning) Flicker(HeatWarning);
        else SetInvisible(HeatWarning);
    }

    private void HandlePressureWarning()
    {
        bool warning = systemManager.pressure >= 80f;

        if (warning) Flicker(PressureWarning);
        else SetInvisible(PressureWarning);
    }

    private void HandleIntegrityWarning()
    {
        bool warning = systemManager.integrity <= 40f;

        if (warning) Flicker(IntegrityWarning);
        else SetInvisible(IntegrityWarning);
    }

    // =========================
    // CRITICAL COLORS
    // =========================

    private void CacheDefaultColors()
    {
        if (powerFill != null) powerDefaultColor = powerFill.color;
        if (heatFill != null) heatDefaultColor = heatFill.color;
        if (pressureFill != null) pressureDefaultColor = pressureFill.color;
        if (integrityFill != null) integrityDefaultColor = integrityFill.color;
    }

    private void UpdateCriticalColors()
    {
        if (systemManager == null) return;

        if (powerFill != null)
        {
            bool critical = systemManager.power <= 10f;
            powerFill.color = critical ? criticalColor : powerDefaultColor;
        }

        if (heatFill != null)
        {
            bool critical =
                systemManager.heat <= systemManager.underheatThreshold ||
                systemManager.heat >= systemManager.overheatThreshold;

            heatFill.color = critical ? criticalColor : heatDefaultColor;
        }

        if (pressureFill != null)
        {
            bool critical = systemManager.pressure >= 100f;
            pressureFill.color = critical ? criticalColor : pressureDefaultColor;
        }

        if (integrityFill != null)
        {
            bool critical = systemManager.integrity <= 20f;
            integrityFill.color = critical ? criticalColor : integrityDefaultColor;
        }
    }

    // =========================
    // FLICKER
    // =========================

    private void Flicker(Image img)
    {
        if (img == null) return;

        float t = Mathf.PingPong(Time.time * flickerSpeed, 1f);
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private void SetInvisible(Image img)
    {
        if (img == null) return;

        Color c = img.color;
        c.a = 0f;
        img.color = c;
    }
}