using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimerUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Settings")]
    [SerializeField] private bool startOnAwake = true;

    private float elapsedTime;
    private bool isRunning;

    void Start()
    {
        if (startOnAwake)
            StartTimer();
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        UpdateUI();
    }

    void UpdateUI()
    {
        timerText.text = $"Time: {elapsedTime:F1}s";
    }

    // =========================================================
    // PUBLIC CONTROLS
    // =========================================================

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateUI();
    }

    public float GetTime()
    {
        return elapsedTime;
    }
}