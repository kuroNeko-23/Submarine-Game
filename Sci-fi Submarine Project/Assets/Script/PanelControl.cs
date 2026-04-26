using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // =========================
    // TOGGLE
    // =========================
    public void TogglePanel()
    {
        bool newState = !panel.activeSelf;
        panel.SetActive(newState);

        PlayClick();
    }

    // =========================
    // OPEN
    // =========================
    public void OpenPanel()
    {
        if (!panel.activeSelf)
        {
            panel.SetActive(true);
            PlayClick();
        }
    }

    // =========================
    // CLOSE
    // =========================
    public void ClosePanel()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
            PlayClick();
        }
    }

    // =========================
    // AUDIO
    // =========================
    private void PlayClick()
    {
        AudioManager.Instance?.PlayMainUIClick();
    }
}