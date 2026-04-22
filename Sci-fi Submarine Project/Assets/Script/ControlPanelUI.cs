using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SystemIntegrityPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private SystemInterferenceManager interferenceManager;
    [SerializeField] private LeakMalfunctionManager leakManager;

    [Header("Sliders")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Slider heatSlider;
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Slider integritySlider;

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

    [Header("Status Config (Editable in Inspector)")]
    [SerializeField] private BinaryStatus electricityConfig;
    [SerializeField] private BinaryStatus panelsConfig;
    [SerializeField] private BinaryStatus reactorConfig;
    [SerializeField] private BinaryStatus heatPipeConfig;
    [SerializeField] private BinaryStatus pressurePipeConfig;

    [Header("Slider Settings")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothSpeed = 5f;

    void Start()
    {
        InitSlider(powerSlider);
        InitSlider(heatSlider);
        InitSlider(pressureSlider);
        InitSlider(integritySlider);
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;
        if (systemManager == null) return;

        UpdateSliders();
        UpdateStatuses();
    }

    // =========================
    // SLIDER LOGIC
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
    }

    private void UpdateSlider(Slider slider, float targetValue)
    {
        if (slider == null) return;

        if (useSmoothing)
        {
            slider.value = Mathf.Lerp(
                slider.value,
                targetValue,
                Time.deltaTime * smoothSpeed
            );
        }
        else
        {
            slider.value = targetValue;
        }
    }

    // =========================
    // STATUS LOGIC (BINARY)
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

        bool isBroken = leakManager.activeLeaks.Contains(LeakMalfunctionManager.LeakType.ReactorLeak);

        reactorStatus.text = isBroken
            ? reactorConfig.errorText
            : reactorConfig.normalText;
    }

    private void UpdateHeatPipe()
    {
        if (heatPipeStatus == null || leakManager == null) return;

        bool isBroken = leakManager.activeLeaks.Contains(LeakMalfunctionManager.LeakType.HeatPipeLeak);

        heatPipeStatus.text = isBroken
            ? heatPipeConfig.errorText
            : heatPipeConfig.normalText;
    }

    private void UpdatePressurePipe()
    {
        if (pressurePipeStatus == null || leakManager == null) return;

        bool isBroken = leakManager.activeLeaks.Contains(LeakMalfunctionManager.LeakType.PressureLeak);

        pressurePipeStatus.text = isBroken
            ? pressurePipeConfig.errorText
            : pressurePipeConfig.normalText;
    }
}