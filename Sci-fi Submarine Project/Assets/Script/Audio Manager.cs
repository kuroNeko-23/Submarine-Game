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
    [SerializeField] private AudioSource heatOverloadAlarm;
    [SerializeField] private AudioSource pressureOverloadAlarm;

    [Header("UI Audio Sources")]
    [SerializeField] private AudioSource mainPanelUIButtonClick;
    [SerializeField] private AudioSource systemPanelUIButtonClick;
    [SerializeField] private AudioSource errorPanelSFX;

    // =========================
    // INIT (Singleton)
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

    public void PlayHeatOverloadAlarm() => PlayLoop(heatOverloadAlarm);
    public void StopHeatOverloadAlarm() => Stop(heatOverloadAlarm);
    public void PlayPressureOverloadAlarm() => PlayLoop(pressureOverloadAlarm);
    public void StopPressureOverloadAlarm() => Stop(pressureOverloadAlarm);
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
}