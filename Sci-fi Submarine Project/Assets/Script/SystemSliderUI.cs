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
        slider.minValue = 0f;
        slider.maxValue = 100f;
    }

    void Update()
    {
        if (systemManager == null || slider == null) return;

        float target = GetSystemValue();

        if (useSmoothing)
        {
            slider.value = Mathf.Lerp(slider.value, target, Time.deltaTime * smoothSpeed);
        }
        else
        {
            slider.value = target;
        }
    }
}