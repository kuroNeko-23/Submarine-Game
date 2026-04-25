using System;
using Unity.VisualScripting;
using UnityEngine;

public class ControlPanelSwitcher : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject systemPanel;
    [SerializeField] private GameObject hullPanel;
    //[SerializeField] private GameObject powerPanel;
    [SerializeField] private GameObject depthPanel;
    [SerializeField] private GameObject logsPanel ;


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

    public void ShowLogs()
    {
        PlayClick();
        HideAll();
        logsPanel.SetActive(true);
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
        logsPanel.SetActive(false);
        depthPanel.SetActive(false);
    }
    private void PlayClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMainPanelUIButton();
    }
}