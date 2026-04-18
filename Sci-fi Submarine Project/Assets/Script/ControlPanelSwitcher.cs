using UnityEngine;

public class ControlPanelSwitcher : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject systemPanel;
    [SerializeField] private GameObject hullPanel;
    [SerializeField] private GameObject powerPanel;

    void Start()
    {
        ShowSystem(); // default panel
    }

    public void ShowSystem()
    {
        HideAll();
        systemPanel.SetActive(true);
    }

    public void ShowHull()
    {
        HideAll();
        hullPanel.SetActive(true);
    }

    public void ShowPower()
    {
        HideAll();
        powerPanel.SetActive(true);
    }

    private void HideAll()
    {
        systemPanel.SetActive(false);
        hullPanel.SetActive(false);
        powerPanel.SetActive(false);
    }
}