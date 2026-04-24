using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Default Shake Settings")]
    public float duration = 0.5f;
    public float magnitude = 0.2f;
    public float frequency = 20f;

    [Header("Debug")]
    public KeyCode debugKey = KeyCode.K;

    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        // Debug trigger
        if (Input.GetKeyDown(debugKey))
        {
            Shake();
        }
    }

    // 🔹 Basic Shake
    public void Shake()
    {
        StartShake(duration, magnitude, frequency);
    }

    // 🔹 Custom Shake (call this from shark attack)
    public void StartShake(float duration, float magnitude, float frequency)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude, frequency));
    }

    // 🔹 Directional Shake (optional - better impact feel)
    public void ShakeDirectional(Vector3 direction, float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeDirectionalRoutine(direction, duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude, float frequency)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Mathf.PerlinNoise(Time.time * frequency, 0f) * 2f - 1f;
            float y = Mathf.PerlinNoise(0f, Time.time * frequency) * 2f - 1f;

            transform.localPosition = originalPos + new Vector3(x, y, 0f) * magnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    IEnumerator ShakeDirectionalRoutine(Vector3 direction, float duration, float magnitude)
    {
        float elapsed = 0f;
        direction.Normalize();

        while (elapsed < duration)
        {
            float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration);
            Vector3 offset = direction * strength;

            transform.localPosition = originalPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}