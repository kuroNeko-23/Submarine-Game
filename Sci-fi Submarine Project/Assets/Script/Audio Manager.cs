using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioSource mainUIClickSFX;

    [Header("Monster Audio Groups")]
    [SerializeField] private AudioSource impactAudioSources;
    [SerializeField] private AudioSource sharkWooshSource;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 3f;

    private Dictionary<AudioSource, float> baseVolumeMap = new Dictionary<AudioSource, float>();
    private Coroutine fadeRoutine;

    // =========================
    // INIT
    // =========================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        CacheBaseVolumes();

        if (ambientAudioSource != null)
        {
            ambientAudioSource.loop = true;
            ambientAudioSource.Play();
        }

        FadeInAllAudio();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CacheBaseVolumes();
        ResetAllAudio();
        FadeInAllAudio();
    }

    // =========================
    // FIX: STABLE BASE VOLUME SYSTEM
    // =========================

    private void CacheBaseVolumes()
    {
        Register(ambientAudioSource);
        Register(reactorLeak);
        Register(heatLeak);
        Register(pressureLeak);
        Register(electricity);

        Register(reactorSound);
        Register(heatSound);
        Register(pressureSound);
        Register(lowPowerAlarm);
        Register(heatOverloadAlarm);
        Register(pressureOverloadAlarm);
        Register(systemFailureAlarm);

        Register(mainPanelUIButtonClick);
        Register(systemPanelUIButtonClick);
        Register(errorPanelSFX);
        Register(mainUIClickSFX);

        Register(impactAudioSources);
        Register(sharkWooshSource);
    }

    private void Register(AudioSource src)
    {
        if (src == null) return;

        if (!baseVolumeMap.ContainsKey(src))
        {
            float vol = src.volume;
            if (vol <= 0.01f) vol = 1f;

            baseVolumeMap[src] = vol;
        }
    }

    private void ResetAllAudio()
    {
        foreach (var kvp in baseVolumeMap)
        {
            if (kvp.Key == null) continue;

            kvp.Key.Stop();
            kvp.Key.time = 0f;
            kvp.Key.volume = kvp.Value;
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

    // =========================
    // UI AUDIO
    // =========================

    public void PlayMainPanelUIButton() => PlayOneShot(mainPanelUIButtonClick);
    public void PlaySystemPanelUIButton() => PlayOneShot(systemPanelUIButtonClick);
    public void PlayErrorPanelSFX() => PlayOneShot(errorPanelSFX);
    public void PlayMainUIClick() => PlayOneShot(mainUIClickSFX);

    // =========================
    // MONSTER AUDIO
    // =========================

    public void PlayImpactSFX() => PlayOneShot(impactAudioSources);
    public void PlaySharkWoosh() => PlayOneShot(sharkWooshSource);

    // =========================
    // HELPERS
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
        if (source.isPlaying) source.Stop();
    }

    private void PlayOneShot(AudioSource source)
    {
        if (source == null || source.clip == null) return;
        source.PlayOneShot(source.clip);
    }

    // =========================
    // FADE SYSTEM (FIXED)
    // =========================

    public void FadeOutAllAudio() => StartFade(1f, 0f);
    public void FadeInAllAudio() => StartFade(0f, 1f);

    private void StartFade(float start, float end)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeAll(start, end));
    }

    private IEnumerator FadeAll(float start, float end)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = t / fadeDuration;

            foreach (var kvp in baseVolumeMap)
            {
                if (kvp.Key == null) continue;

                float baseVol = kvp.Value;
                kvp.Key.volume = Mathf.Lerp(baseVol * start, baseVol * end, lerp);
            }

            yield return null;
        }

        foreach (var kvp in baseVolumeMap)
        {
            if (kvp.Key == null) continue;

            kvp.Key.volume = kvp.Value * end;
        }
    }
}