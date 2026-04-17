using UnityEngine;
using UnityEngine.UI;

public class SystemIntegrityPanelUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SystemManager systemManager;

    [Header("Sliders")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Slider heatSlider;
    [SerializeField] private Slider pressureSlider;
    [SerializeField] private Slider integritySlider;

    [Header("Settings")]
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
        if (systemManager == null) return;

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
}