using UnityEngine;
using UnityEngine.Splines;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

public enum SplineType
{
    Normal,
    WindowPass,
    Attack
}

[System.Serializable]
public class SplineData
{
    public string id;
    public SplineContainer spline;
    public float speed = 0.1f;
    public SplineType type;
}

public class MegalodonSplineController : MonoBehaviour
{
    public enum MovementMode
    {
        Loop,
        OneShot
    }

    [Header("Spline Library")]
    [SerializeField] private SplineData[] splines;

    [Header("Attack Settings")]
    [SerializeField, Range(0f, 1f)]
    private float attackTriggerT = 0.4f;

    [SerializeField] private float exitSpeedMultiplier = 0.3f;

    [Header("Attack Effects")]
    [SerializeField] private Animator animator;
    [SerializeField] private CameraShake cameraShake;

    [Header("System Damage")]
    [SerializeField] private LeakMalfunctionManager leakManager;
    [SerializeField] private SystemInterferenceManager systemManager;

    [SerializeField] private int malfunctionMode = 1;

    public event Action OnSplineFinished;

    private SplineContainer currentSpline;
    private SplineType currentType;

    private float t;
    private float speed;

    private MovementMode mode;

    private bool finished = false;
    private bool attackTriggered = false;
    private bool exitTriggered = false;

    private Vector3 lastTangent = Vector3.forward;

    void Update()
    {
        if (currentSpline == null) return;
        Move();
    }

    private void Move()
    {
        float s = speed;

        if (mode == MovementMode.OneShot && exitTriggered)
            s *= exitSpeedMultiplier;

        t += s * Time.deltaTime;

        if (mode == MovementMode.Loop)
        {
            if (t > 1f) t -= 1f;
            if (t < 0f) t += 1f;
        }

        t = Mathf.Clamp01(t);

        // =========================
        // ATTACK TRIGGER
        // =========================
        if (mode == MovementMode.OneShot &&
            currentType == SplineType.Attack &&
            !attackTriggered &&
            t >= attackTriggerT)
        {
            attackTriggered = true;
            exitTriggered = true;

            TriggerAttackEvent();
        }

        Vector3 pos = currentSpline.EvaluatePosition(t);
        Vector3 tangent = currentSpline.EvaluateTangent(t);

        if (tangent.sqrMagnitude < 0.00001f)
            tangent = lastTangent;
        else
        {
            tangent.Normalize();
            lastTangent = tangent;
        }

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(tangent);

        if (mode == MovementMode.OneShot && t >= 1f && !finished)
        {
            finished = true;
            OnSplineFinished?.Invoke();
        }
    }

    // =========================
    // PLAY API (CLEAN)
    // =========================

    public void PlayFarRight() => Play("Far_Right");
    public void PlayFarLeft() => Play("Far_Left");

    public void PlayCloseRight() => Play("Close_Right");
    public void PlayCloseLeft() => Play("Close_Left");

    public void PlayWindowPassRight() => Play("WindowPass_Right");
    public void PlayWindowPassLeft() => Play("WindowPass_Left");

    public void PlayAttack() => Play("Attack");

    public void PlayLoop(string id)
    {
        var s = Get(id);
        if (s == null) return;

        currentSpline = s.spline;
        speed = s.speed;
        currentType = s.type;

        mode = MovementMode.Loop;

        t = 0f;
        ResetState();
    }

    public void PlayOneShot(string id)
    {
        var s = Get(id);
        if (s == null) return;

        currentSpline = s.spline;
        speed = s.speed;
        currentType = s.type;

        mode = MovementMode.OneShot;

        t = 0f;
        ResetState();
    }

    private void Play(string id)
    {
        PlayOneShot(id);
    }

    private SplineData Get(string id)
    {
        foreach (var s in splines)
            if (s.id == id)
                return s;

        Debug.LogWarning($"Spline not found: {id}");
        return null;
    }

    private void ResetState()
    {
        finished = false;
        attackTriggered = false;
        exitTriggered = false;
    }

    // =========================
    // ATTACK EVENT
    // =========================

    private void TriggerAttackEvent()
    {
        Debug.Log("🦈 ATTACK TRIGGERED");

        if (animator != null)
            animator.SetTrigger("Attack");

        if (cameraShake != null)
            cameraShake.Shake();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayImpactSFX();

        TriggerSystemBreak();
    }

    private void TriggerSystemBreak()
    {
        List<System.Action> pool = new List<System.Action>();

        if (leakManager != null)
            pool.Add(() => leakManager.TriggerRandomLeak());

        if (systemManager != null)
            pool.Add(() => systemManager.TriggerRandomInterference());

        if (pool.Count == 0) return;

        if (malfunctionMode >= 3)
        {
            foreach (var p in pool)
                p.Invoke();
            return;
        }

        if (malfunctionMode == 2)
        {
            for (int i = 0; i < Mathf.Min(2, pool.Count); i++)
            {
                int index = UnityEngine.Random.Range(0, pool.Count);
                pool[index].Invoke();
                pool.RemoveAt(index);
            }
            return;
        }

        int single = UnityEngine.Random.Range(0, pool.Count);
        pool[single].Invoke();
    }
}