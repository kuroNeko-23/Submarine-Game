using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IntegrityHoldToFix : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Reference")]
    [SerializeField] private SystemManager systemManager;

    [Header("UI")]
    [SerializeField] private Image progressCircle;

    [Header("Hold Settings")]
    [SerializeField] private float holdDuration = 2f;

    private float holdTimer = 0f;
    private bool isHolding = false;

    void Update()
    {
        if (!isHolding) return;

        // ❌ Stop if system is not in failure
        if (systemManager == null || !systemManager.isLocked)
        {
            ResetProgress();
            return;
        }

        holdTimer += Time.deltaTime;

        float progress = holdTimer / holdDuration;

        if (progressCircle != null)
            progressCircle.fillAmount = progress;

        if (holdTimer >= holdDuration)
        {
            CompleteFix();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (systemManager == null || !systemManager.isLocked)
            return;

        isHolding = true;
        holdTimer = 0f;

        if (progressCircle != null)
            progressCircle.fillAmount = 0f;
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
        ResetProgress();

        if (systemManager != null)
        {
            systemManager.ResolveFailure(); // 🔥 single call
            Debug.Log("🔧 Integrity Resolved");
        }
    }

    private void ResetProgress()
    {
        isHolding = false;
        holdTimer = 0f;

        if (progressCircle != null)
            progressCircle.fillAmount = 0f;
    }
}