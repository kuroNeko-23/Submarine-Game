using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance;

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // GENERAL UI CLICKS
    // =========================

    public void PlayClick()
    {
        AudioManager.Instance?.PlayMainUIClick();
    }

    public void PlayMainButtonClick()
    {
        AudioManager.Instance?.PlayMainPanelUIButton();
    }

    public void PlaySystemButtonClick()
    {
        AudioManager.Instance?.PlaySystemPanelUIButton();
    }

    public void PlayError()
    {
        AudioManager.Instance?.PlayErrorPanelSFX();
    }

    // =========================
    // SYSTEM SOUND UI (your specific ones)
    // =========================

    public void PlayReactorSound()
    {
        AudioManager.Instance?.PlayReactor();
    }

    public void StopReactorSound()
    {
        AudioManager.Instance?.StopReactor();
    }

    public void PlayHeatSound()
    {
        AudioManager.Instance?.PlayHeat();
    }

    public void StopHeatSound()
    {
        AudioManager.Instance?.StopHeat();
    }

    public void PlayPressureSound()
    {
        AudioManager.Instance?.PlayPressure();
    }

    // =========================
    // OPTIONAL: SHARED UI FEEDBACK
    // =========================

    public void PlayImpact()
    {
        AudioManager.Instance?.PlayImpactSFX();
    }

    public void PlaySharkWoosh()
    {
        AudioManager.Instance?.PlaySharkWoosh();
    }
}