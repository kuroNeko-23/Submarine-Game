using UnityEngine;
using TMPro;

public class RepairPromptUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text holdToFixText;

    [Header("References")]
    [SerializeField] private SystemInterferenceManager interferenceManager;
    [SerializeField] private LeakMalfunctionManager leakManager;
    [SerializeField] private SystemManager systemManager;

    void Update()
    {
        if (ShouldShowPrompt())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private bool ShouldShowPrompt()
    {
        // Electricity fix
        if (interferenceManager != null &&
            interferenceManager.currentMalfunction != SystemInterferenceManager.SystemType.None)
            return true;

        // Leak fix
        if (leakManager != null &&
            leakManager.activeLeaks.Count > 0)
            return true;

        // Integrity fix
        if (systemManager != null &&
            systemManager.isLocked)
            return true;

        return false;
    }

    private void Show()
    {
        if (holdToFixText == null) return;

        holdToFixText.gameObject.SetActive(true);
    }

    private void Hide()
    {
        if (holdToFixText == null) return;

        holdToFixText.gameObject.SetActive(false);
    }
}