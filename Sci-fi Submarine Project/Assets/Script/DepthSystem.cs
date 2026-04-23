using UnityEngine;
using TMPro;
using System;

public class DepthSystem : MonoBehaviour
{
    [Header("Depth Settings")]
    [SerializeField] private float currentDepth = 0f;      // meters
    [SerializeField] private float descentSpeed = 5f;      // meters per second
    [SerializeField] private float maxDepth = 11000f;      // Mariana trench ~11km

    [Header("Difficulty Scaling")]
    [SerializeField] private AnimationCurve depthDifficultyCurve;
    // X = normalized depth (0–1), Y = difficulty multiplier

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI depthValueText;

    // Events (for other systems like pressure, heat, etc.)
    public static Action<float> OnDepthChanged;
    public static Action<float> OnDifficultyChanged;

    private float difficultyMultiplier;

    public float CurrentDepth => currentDepth;
    public float DifficultyMultiplier => difficultyMultiplier;

    void Update()
    {
        UpdateDepth();
        UpdateDifficulty();
        UpdateUI();
    }

    void UpdateDepth()
    {
        currentDepth += descentSpeed * Time.deltaTime;
        currentDepth = Mathf.Clamp(currentDepth, 0f, maxDepth);

        OnDepthChanged?.Invoke(currentDepth);
    }

    void UpdateDifficulty()
    {
        float normalizedDepth = currentDepth / maxDepth;

        difficultyMultiplier = depthDifficultyCurve.Evaluate(normalizedDepth);

        OnDifficultyChanged?.Invoke(difficultyMultiplier);
    }

    void UpdateUI()
    {
        if (depthValueText != null)
        {
            depthValueText.text = Mathf.FloorToInt(currentDepth) + " m";
        }
    }
}