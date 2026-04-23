using System;
using UnityEngine;

public class ControlPanelSwitcher : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject systemPanel;
    [SerializeField] private GameObject hullPanel;
    [SerializeField] private GameObject powerPanel;
    [SerializeField] private GameObject depthPanel;


    void Start()
    {
        ShowSystem(); // default panel
    }

    public void ShowSystem()
    {
        PlayClick();
        HideAll();
        systemPanel.SetActive(true);
    }

    public void ShowHull()
    {
        PlayClick();
        HideAll();
        hullPanel.SetActive(true);
    }

    public void ShowPower()
    {
        PlayClick();
        HideAll();
        powerPanel.SetActive(true);
    }
    public void ShowDepth()
    {
        PlayClick();
        HideAll();
        depthPanel.SetActive(true);
    }

    private void HideAll()
    {
        systemPanel.SetActive(false);
        hullPanel.SetActive(false);
        powerPanel.SetActive(false);
        depthPanel.SetActive(false);
    }
    private void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMainPanelUIButton();
    }
}