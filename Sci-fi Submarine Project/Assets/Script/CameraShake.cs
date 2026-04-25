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

    private Coroutine shakeCoroutine;

    // IMPORTANT: we no longer store absolute position
    private Vector3 shakeOffset;

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            Shake();
        }
    }

    public void Shake()
    {
        StartShake(duration, magnitude, frequency);
    }

    public void StartShake(float duration, float magnitude, float frequency)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude, frequency));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude, float frequency)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Mathf.PerlinNoise(Time.time * frequency, 0f) * 2f - 1f;
            float y = Mathf.PerlinNoise(0f, Time.time * frequency) * 2f - 1f;

            shakeOffset = new Vector3(x, y, 0f) * magnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    // 🔥 THIS is what fixes your issue
    public Vector3 GetShakeOffset()
    {
        return shakeOffset;
    }
}