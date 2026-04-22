using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldToFix : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Managers")]
    [SerializeField] private SystemInterferenceManager interferenceManager;
    [SerializeField] private LeakMalfunctionManager leakManager;

    [Header("Leak Fix Type (ONLY for leak panels)")]
    [SerializeField] private LeakMalfunctionManager.LeakType fixType;

    [Header("UI")]
    [SerializeField] private Image progressCircle;

    [Header("Settings")]
    [SerializeField] private float holdDuration = 2f;

    private float holdTimer = 0f;
    private bool isHolding = false;

    void Update()
    {
        if (!isHolding) return;

        holdTimer += Time.deltaTime;

        float progress = holdTimer / holdDuration;
        progressCircle.fillAmount = progress;

        if (holdTimer >= holdDuration)
        {
            CompleteFix();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetProgress();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetProgress();
    }

    private void CompleteFix()
    {
        isHolding = false;
        holdTimer = 0f;
        progressCircle.fillAmount = 0f;

        // =========================
        // FIX LOGIC
        // =========================

        // 1. Try fixing leak first (if assigned)
        if (leakManager != null && fixType != LeakMalfunctionManager.LeakType.None)
        {
            leakManager.ResolveLeak(fixType);
            Debug.Log($"🔧 Leak Fixed: {fixType}");
            return;
        }

        // 2. Otherwise fix electricity/system interference
        if (interferenceManager != null)
        {
            interferenceManager.ResolveMalfunction();
            Debug.Log("🔧 Electricity/System Fixed");
        }
    }

    private void ResetProgress()
    {
        isHolding = false;
        holdTimer = 0f;
        progressCircle.fillAmount = 0f;
    }
}