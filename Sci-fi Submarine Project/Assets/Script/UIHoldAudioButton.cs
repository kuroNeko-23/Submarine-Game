using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoldAudioButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum AudioType
    {
        Reactor,
        Heat,
        PressureOneShot
    }

    [SerializeField] private AudioType audioType;

    private bool isHolding;

    // =========================
    // POINTER DOWN
    // =========================
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;

        switch (audioType)
        {
            case AudioType.Reactor:
                AudioManager.Instance?.PlayReactor();
                break;

            case AudioType.Heat:
                AudioManager.Instance?.PlayHeat();
                break;

            case AudioType.PressureOneShot:
                AudioManager.Instance?.PlayPressure();
                break;
        }
    }

    // =========================
    // POINTER UP
    // =========================
    public void OnPointerUp(PointerEventData eventData)
    {
        StopHold();
    }

    // =========================
    // POINTER EXIT (important safety)
    // =========================
    public void OnPointerExit(PointerEventData eventData)
    {
        StopHold();
    }

    // =========================
    // STOP LOGIC
    // =========================
    private void StopHold()
    {
        if (!isHolding) return;
        isHolding = false;

        switch (audioType)
        {
            case AudioType.Reactor:
                AudioManager.Instance?.StopReactor();
                break;

            case AudioType.Heat:
                AudioManager.Instance?.StopHeat();
                break;

            case AudioType.PressureOneShot:
                // nothing to stop (one-shot)
                break;
        }
    }
}