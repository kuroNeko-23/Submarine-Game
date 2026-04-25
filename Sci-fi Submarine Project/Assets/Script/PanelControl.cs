using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Call this to toggle the panel
    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    // Explicit open
    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    // Explicit close
    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}