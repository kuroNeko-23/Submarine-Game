using UnityEngine;
using UnityEngine.UI;

public class SystemSliderUI : MonoBehaviour
{
    public enum SystemType
    {
        Power,
        Heat,
        Pressure,
        Integrity
    }

    [SerializeField] private SystemManager systemManager;
    [SerializeField] private Slider slider;
    [SerializeField] private SystemType systemType;

    [Header("Fill")]
    [SerializeField] private Image fillImage;

    [Header("Critical Color")]
    [SerializeField] private Color criticalColor = Color.red;

    private Color defaultColor;

    [Header("Smoothing")]
    [SerializeField] private bool useSmoothing = true;
    [SerializeField] private float smoothSpeed = 5f;

    private float GetSystemValue()
    {
        switch (systemType)
        {
            case SystemType.Power: return systemManager.power;
            case SystemType.Heat: return systemManager.heat;
            case SystemType.Pressure: return systemManager.pressure;
            case SystemType.Integrity: return systemManager.integrity;
        }

        return 0f;
    }

    void Start()
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.interactable = false;
        }

        // Cache default color
        if (fillImage != null)
        {
            defaultColor = fillImage.color;
        }
    }

    void Update()
    {
        if (systemManager == null || slider == null) return;

        float target = GetSystemValue();

        // Slider value
        if (useSmoothing)
        {
            slider.value = Mathf.Lerp(slider.value, target, Time.deltaTime * smoothSpeed);
        }
        else
        {
            slider.value = target;
        }

        // 🔴 Critical Color Update
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (fillImage == null || systemManager == null) return;

        bool isCritical = false;

        switch (systemType)
        {
            case SystemType.Power:
                isCritical = systemManager.power <= 10f;
                break;

            case SystemType.Heat:
                isCritical =
                    systemManager.heat <= systemManager.underheatThreshold ||
                    systemManager.heat >= systemManager.overheatThreshold;
                break;

            case SystemType.Pressure:
                isCritical = systemManager.pressure >= 100f;
                break;

            case SystemType.Integrity:
                isCritical = systemManager.integrity <= 20f;
                break;
        }

        fillImage.color = isCritical ? criticalColor : defaultColor;
    }
}