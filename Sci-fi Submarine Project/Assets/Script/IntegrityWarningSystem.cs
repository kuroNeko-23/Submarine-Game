using System.Collections;
using UnityEngine;

public class IntegrityWarningSystem : MonoBehaviour
{
    [System.Serializable]
    public class ScreenMaterialTarget
    {
        public Renderer renderer;
        public int materialIndex = 0;
    }

    [Header("Integrity Reference")]
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private float lowIntegrityThreshold = 30f;

    [Header("Audio Warning")]
    [SerializeField] private AudioSource warningAudio;
    [SerializeField] private float warningInterval = 2f;

    [Header("Screen Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material warningMaterial;
    [SerializeField] private ScreenMaterialTarget[] screenMaterialTargets;

    [Header("Emergency Lights")]
    [SerializeField] private Light[] emergencyLights;

    private Coroutine warningRoutine;
    private bool isWarningActive;

    private void Update()
    {
        if (systemManager == null) return;

        bool shouldWarn = systemManager.integrity <= lowIntegrityThreshold;

        if (shouldWarn && !isWarningActive)
            StartWarning();

        if (!shouldWarn && isWarningActive)
            StopWarning();
    }

    // =========================
    // WARNING CONTROL
    // =========================

    private void StartWarning()
    {
        isWarningActive = true;

        ApplyMaterialWarning();
        EnableEmergencyLights();

        if (warningRoutine != null)
            StopCoroutine(warningRoutine);

        warningRoutine = StartCoroutine(WarningAudioLoop());
    }

    private void StopWarning()
    {
        isWarningActive = false;

        RestoreNormalMaterials();
        DisableEmergencyLights();

        if (warningRoutine != null)
            StopCoroutine(warningRoutine);

        if (warningAudio != null)
            warningAudio.Stop();
    }

    // =========================
    // AUDIO LOOP
    // =========================

    private IEnumerator WarningAudioLoop()
    {
        while (isWarningActive)
        {
            if (warningAudio != null)
                warningAudio.Play();

            yield return new WaitForSeconds(warningInterval);
        }
    }

    // =========================
    // MATERIAL SWAP SYSTEM
    // =========================

    private void ApplyMaterialWarning()
    {
        ApplyMaterialToAll(warningMaterial);
    }

    private void RestoreNormalMaterials()
    {
        ApplyMaterialToAll(normalMaterial);
    }

    private void ApplyMaterialToAll(Material mat)
    {
        if (screenMaterialTargets == null) return;

        foreach (var target in screenMaterialTargets)
        {
            if (target.renderer == null) continue;

            Material[] mats = target.renderer.sharedMaterials;

            if (target.materialIndex < 0 || target.materialIndex >= mats.Length)
                continue;

            mats[target.materialIndex] = mat;
            target.renderer.sharedMaterials = mats;
        }
    }

    // =========================
    // LIGHTS
    // =========================

    private void EnableEmergencyLights()
    {
        if (emergencyLights == null) return;

        foreach (var l in emergencyLights)
        {
            if (l != null)
                l.enabled = true;
        }
    }

    private void DisableEmergencyLights()
    {
        if (emergencyLights == null) return;

        foreach (var l in emergencyLights)
        {
            if (l != null)
                l.enabled = false;
        }
    }
}