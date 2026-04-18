using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoldToFix : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("System Interference Manager")]
    [SerializeField] private SystemInterferenceManager interferenceManager;
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

        Debug.Log("Electricity Fixed!");

        // 🔥 ACTUAL FIX
        if (interferenceManager != null)
        {
            interferenceManager.ResolveMalfunction();
        }
    }

    private void ResetProgress()
    {
        isHolding = false;
        holdTimer = 0f;
        progressCircle.fillAmount = 0f;
    }
}