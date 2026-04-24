using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Light References")]
    [SerializeField] private Light[] targetLights;

    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 5f;
    [SerializeField] private float flickerSpeed = 1.5f;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private float noiseOffset;

    void Awake()
    {
        noiseOffset = Random.Range(0f, 100f);

        // 🚨 start OFF
        SetLights(false);
    }

    void Update()
    {
        if (!isActive || targetLights == null) return;

        float time = Time.time * flickerSpeed + noiseOffset;

        // smooth unstable breathing flicker
        float wave = Mathf.Sin(time);
        float noise = Mathf.PerlinNoise(time, 0f) - 0.5f;

        float combined = wave + noise * 0.4f;

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (combined + 1f) * 0.5f);

        SetIntensity(intensity);
    }

    // =========================
    // CONTROL
    // =========================

    public void StartFlicker()
    {
        isActive = true;
        SetLights(true); // turn ON lights
    }

    public void StopFlicker()
    {
        isActive = false;
        SetLights(false); // turn OFF lights
    }

    // =========================
    // INTERNAL HELPERS
    // =========================

    private void SetLights(bool state)
    {
        for (int i = 0; i < targetLights.Length; i++)
        {
            if (targetLights[i] == null) continue;
            targetLights[i].enabled = state;
        }
    }

    private void SetIntensity(float value)
    {
        for (int i = 0; i < targetLights.Length; i++)
        {
            if (targetLights[i] == null) continue;
            targetLights[i].intensity = value;
        }
    }
}