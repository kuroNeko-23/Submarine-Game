using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SharkRadarController : MonoBehaviour
{
    public enum RadarState
    {
        None,
        Far,
        Close,
        Attack
    }

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip radarClip;

    [Header("UI (Optional)")]
    [SerializeField] private Image radarDot; // assign a small UI circle

    [Header("Timing")]
    [SerializeField] private float farInterval = 2.5f;
    [SerializeField] private float closeInterval = 1.2f;
    [SerializeField] private float attackInterval = 0.4f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    private Coroutine fadeRoutine;

    private RadarState currentState = RadarState.None;
    private Coroutine radarRoutine;

    // =========================
    // PUBLIC API
    // =========================
    public void SetRadarState(RadarState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (radarRoutine != null)
            StopCoroutine(radarRoutine);

        if (currentState == RadarState.None)
        {
            if (radarRoutine != null)
                StopCoroutine(radarRoutine);

            radarRoutine = null;

            // 🔥 FADE OUT INSTEAD OF HARD STOP
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeOutAudio());

            SetUIDot(false);
            return;
        }

        radarRoutine = StartCoroutine(RadarLoop());
    }
    IEnumerator FadeOutAudio()
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset for next play
    }

    // =========================
    // CORE LOOP
    // =========================
    private IEnumerator RadarLoop()
    {
        while (true)
        {
            float interval = GetInterval(currentState);

            // 🔊 Play sound
            if (audioSource != null && radarClip != null)
            {
                audioSource.PlayOneShot(radarClip);
            }

            // 🔴 Blink UI
            StartCoroutine(BlinkDot());

            yield return new WaitForSeconds(interval);
        }
    }

    // =========================
    // HELPERS
    // =========================
    private float GetInterval(RadarState state)
    {
        switch (state)
        {
            case RadarState.Far: return farInterval;
            case RadarState.Close: return closeInterval;
            case RadarState.Attack: return attackInterval;
            default: return 2f;
        }
    }

    private IEnumerator BlinkDot()
    {
        if (radarDot == null) yield break;

        SetUIDot(true);
        yield return new WaitForSeconds(0.1f);
        SetUIDot(false);
    }

    private void SetUIDot(bool state)
    {
        if (radarDot != null)
            radarDot.enabled = state;
    }
}