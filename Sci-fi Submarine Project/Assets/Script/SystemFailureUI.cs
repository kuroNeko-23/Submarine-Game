using UnityEngine;

public class SystemFailureUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private GameObject failurePanel;

    [Header("Settings")]
    [SerializeField] private bool hideWhenResolved = true;

    private bool wasLocked = false;

    void Update()
    {
        if (systemManager == null || failurePanel == null) return;

        bool isLocked = systemManager.isLocked;

        // Trigger only when state changes
        if (isLocked && !wasLocked)
        {
            ShowFailurePanel();
        }
        else if (!isLocked && wasLocked)
        {
            if (hideWhenResolved)
                HideFailurePanel();
        }

        wasLocked = isLocked;
    }

    private void ShowFailurePanel()
    {
        failurePanel.SetActive(true);
        Debug.Log("🚨 SYSTEM FAILURE PANEL SHOWN");
    }

    private void HideFailurePanel()
    {
        failurePanel.SetActive(false);
        Debug.Log("✅ SYSTEM FAILURE PANEL HIDDEN");
    }
}