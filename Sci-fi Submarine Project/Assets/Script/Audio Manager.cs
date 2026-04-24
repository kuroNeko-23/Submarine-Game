using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ambient")]
    [SerializeField] private AudioSource ambientAudioSource;

    [Header("Malfunction Audio Sources")]
    [SerializeField] private AudioSource reactorLeak;
    [SerializeField] private AudioSource heatLeak;
    [SerializeField] private AudioSource pressureLeak;
    [SerializeField] private AudioSource electricity;

    [Header("Core System Audio Sources")]
    [SerializeField] private AudioSource reactorSound;
    [SerializeField] private AudioSource heatSound;
    [SerializeField] private AudioSource pressureSound;
    [SerializeField] private AudioSource lowPowerAlarm;
    [SerializeField] private AudioSource heatOverloadAlarm;
    [SerializeField] private AudioSource pressureOverloadAlarm;
    [SerializeField] private AudioSource systemFailureAlarm;
    [Header("UI Audio Sources")]
    [SerializeField] private AudioSource mainPanelUIButtonClick;
    [SerializeField] private AudioSource systemPanelUIButtonClick;
    [SerializeField] private AudioSource errorPanelSFX;

     [Header("Master Audio Groups")]
    [SerializeField] private AudioSource[] allGameAudioSources;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 3f;

    // =========================
    // INIT (Singleton)
    // =========================

    private void Awake()
    {
        allGameAudioSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        

    }

    void Start()
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource.loop = true;
            ambientAudioSource.Play();
        }
    }

    // =========================
    // MALFUNCTION AUDIO
    // =========================

    public void PlayReactorLeak() => PlayLoop(reactorLeak);
    public void StopReactorLeak() => Stop(reactorLeak);

    public void PlayHeatLeak() => PlayLoop(heatLeak);
    public void StopHeatLeak() => Stop(heatLeak);

    public void PlayPressureLeak() => PlayLoop(pressureLeak);
    public void StopPressureLeak() => Stop(pressureLeak);

    public void PlayElectricity() => PlayLoop(electricity);
    public void StopElectricity() => Stop(electricity);

    // =========================
    // CORE SYSTEM AUDIO
    // =========================

    public void PlayReactor() => PlayLoop(reactorSound);
    public void StopReactor() => Stop(reactorSound);

    public void PlayHeat() => PlayLoop(heatSound);
    public void StopHeat() => Stop(heatSound);

    public void PlayPressure() => PlayOneShot(pressureSound);
    
    public void PlayLowPowerAlarm() => PlayLoop(lowPowerAlarm);
    public void StopLowPowerAlarm() => Stop(lowPowerAlarm);

    public void PlayHeatOverloadAlarm() => PlayLoop(heatOverloadAlarm);
    public void StopHeatOverloadAlarm() => Stop(heatOverloadAlarm);
    public void PlayPressureOverloadAlarm() => PlayLoop(pressureOverloadAlarm);
    public void StopPressureOverloadAlarm() => Stop(pressureOverloadAlarm);
    public void PlaySystemFailureAlarm() => PlayLoop(systemFailureAlarm);
    public void StopSystemFailureAlarm() => Stop(systemFailureAlarm);
    //public void StopPressure() => Stop(pressureSound);

    // =========================
    // UI AUDIO
    // =========================

    public void PlayMainPanelUIButton()
    {
        PlayOneShot(mainPanelUIButtonClick);
    }
    public void PlaySystemPanelUIButton()
    {
        PlayOneShot(systemPanelUIButtonClick);
    }
    public void PlayErrorPanelSFX()
    {
        PlayOneShot(errorPanelSFX);
    }
    // =========================
    // GENERIC HELPERS
    // =========================

    private void PlayLoop(AudioSource source)
    {
        if (source == null) return;

        if (!source.isPlaying)
        {
            source.loop = true;
            source.Play();
        }
    }

    private void Stop(AudioSource source)
    {
        if (source == null) return;

        if (source.isPlaying)
        {
            source.Stop();
        }
    }

    private void PlayOneShot(AudioSource source)
    {
        if (source == null) return;

        source.PlayOneShot(source.clip);
    }
    // All Audio Control
    public void FadeOutAllAudio()
    {
        StartCoroutine(FadeAll(1f, 0f));
    }

    public void FadeInAllAudio()
    {
        StartCoroutine(FadeAll(0f, 1f));
    }
    private IEnumerator FadeAll(float start, float end)
    {
        float t = 0f;

        // cache starting volumes
        float[] originalVolumes = new float[allGameAudioSources.Length];

        for (int i = 0; i < allGameAudioSources.Length; i++)
        {
            if (allGameAudioSources[i] == null) continue;
            originalVolumes[i] = allGameAudioSources[i].volume;
        }

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / fadeDuration;

            for (int i = 0; i < allGameAudioSources.Length; i++)
            {
                if (allGameAudioSources[i] == null) continue;

                allGameAudioSources[i].volume = Mathf.Lerp(
                    originalVolumes[i] * start,
                    originalVolumes[i] * end,
                    lerp
                );
            }

            yield return null;
        }

        // ensure final state
        for (int i = 0; i < allGameAudioSources.Length; i++)
        {
            if (allGameAudioSources[i] == null) continue;
            allGameAudioSources[i].volume = originalVolumes[i] * end;
        }
    }

}