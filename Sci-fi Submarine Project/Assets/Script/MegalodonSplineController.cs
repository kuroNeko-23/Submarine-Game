using UnityEngine;
using UnityEngine.Splines;
using Sirenix.OdinInspector;

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
    public SplineType type; // 🔥 IMPORTANT
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
    [SerializeField] private float attackTriggerT = 0.4f;     
    [SerializeField] private float exitSpeedMultiplier = 0.3f;
    [Header("Window Pass Settings")]
    [SerializeField] private float windowPassTriggerT = 0.3f; // when woosh happens
    private bool hasTriggeredWindowPass = false;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private MegalodonManager manager;
    [SerializeField] private CameraShake cameraShake;
    

    private SplineContainer currentSpline;
    private SplineType currentType = SplineType.Normal;

    private float t;
    private float currentSpeed;
    private int direction = 1;

    private MovementMode mode = MovementMode.Loop;

    private bool hasTriggeredAttack = false;
    private bool isExiting = false;
    private bool hasFinished = false;

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        if (currentSpline == null) return;

        Move();
    }

    // =========================
    // CORE MOVEMENT
    // =========================

    private void Move()
    {
        float speedToUse = currentSpeed;

        // 🔥 Slow exit after attack hit
        if (mode == MovementMode.OneShot && isExiting)
        {
            speedToUse *= exitSpeedMultiplier;
        }

        t += speedToUse * direction * Time.deltaTime;
        // =========================
        // WINDOW PASS TRIGGER
        // =========================
        if (mode == MovementMode.OneShot
            && currentType == SplineType.WindowPass
            && !hasTriggeredWindowPass
            && t >= windowPassTriggerT)
        {
            hasTriggeredWindowPass = true;
            TriggerWindowPassEvent();
        }
        

        // =========================
        // ATTACK TRIGGER ONLY FOR ATTACK TYPE
        // =========================
        if (mode == MovementMode.OneShot
            && currentType == SplineType.Attack
            && !hasTriggeredAttack
            && t >= attackTriggerT)
        {
            hasTriggeredAttack = true;
            isExiting = true;

            TriggerAttackEvent();
        }

        // =========================
        // LOOP MODE
        // =========================
        if (mode == MovementMode.Loop)
        {
            if (t > 1f) t -= 1f;
            else if (t < 0f) t += 1f;
        }

        // =========================
        // ONE SHOT END
        // =========================
        if (mode == MovementMode.OneShot && t >= 1f && !hasFinished)
        {
            hasFinished = true;
            OnOneShotFinished();
            return;
        }

        // =========================
        // APPLY TRANSFORM
        // =========================
        Vector3 pos = currentSpline.EvaluatePosition(t);
        Vector3 tangent = currentSpline.EvaluateTangent(t) * direction;

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(tangent);
    }

    // =========================
    // SPLINE LOOKUP
    // =========================

    private SplineData GetSpline(string id)
    {
        foreach (var s in splines)
        {
            if (s.id == id)
                return s;
        }

        Debug.LogWarning($"Spline not found: {id}");
        return null;
    }

    // =========================
    // LOOP CONTROL
    // =========================

    public void PlayLoop(string id)
    {
        var s = GetSpline(id);
        if (s == null) return;

        currentSpline = s.spline;
        currentSpeed = s.speed;
        currentType = s.type;

        mode = MovementMode.Loop;

        t = FindClosestTOnSpline(currentSpline, transform.position);

        ResetState();
    }

    // =========================
    // ONE SHOT (ATTACK / PASS)
    // =========================

    public void PlayOneShot(string id)
    {
        var s = GetSpline(id);
        if (s == null) return;

        currentSpline = s.spline;
        currentSpeed = s.speed;
        currentType = s.type;

        mode = MovementMode.OneShot;

        direction = 1;
        t = 0f;

        ResetState();
    }

    private void OnOneShotFinished()
    {
        // 🔥 ONLY DESPAWN IF ATTACK
        if (currentType == SplineType.Attack)
        {
            Debug.Log("🦈 Attack Finished");

            if (manager != null)
                manager.Despawn();
        }
    }

    private void ResetState()
    {
        hasTriggeredAttack = false;
        hasTriggeredWindowPass = false; // 🔥 NEW
        isExiting = false;
        hasFinished = false;
    }

    // =========================
    // DIRECTION CONTROL
    // =========================

    public void SetDirectionForward() => direction = 1;
    public void SetDirectionBackward() => direction = -1;
    public void ReverseDirection() => direction *= -1;

    public void SetDirection(int dir)
    {
        direction = Mathf.Sign(dir) >= 0 ? 1 : -1;
    }

    // =========================
    // ATTACK EVENT
    // =========================

    private void TriggerAttackEvent()
    {
        Debug.Log("🦈 ATTACK TRIGGERED");

        // 🎬 Animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 🎥 Camera Shake
        if (cameraShake != null)
        {
            cameraShake.Shake();
        }

        // 🔊 Audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayImpactSFX();
        }
    }
    /// <summary>
    /// Window Pass Event - called when passing by the window (not attack)  
    /// </summary>
    private void TriggerWindowPassEvent()
    {
        Debug.Log("🦈 WINDOW PASS WOOSH");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySharkWoosh();
        }
    }

    // =========================
    // UTILITY
    // =========================

    private float FindClosestTOnSpline(SplineContainer spline, Vector3 worldPos)
    {
        float bestT = 0f;
        float bestDist = float.MaxValue;

        int resolution = 50;

        for (int i = 0; i <= resolution; i++)
        {
            float tSample = i / (float)resolution;
            Vector3 point = spline.EvaluatePosition(tSample);

            float dist = (point - worldPos).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = tSample;
            }
        }

        return bestT;
    }

    // =========================
    // DEBUG
    // =========================

    [Button("▶ Far Forward")]
    private void DebugFarForward()
    {
        PlayLoop("Far");
        SetDirectionForward();
    }

    [Button("◀ Far Backward")]
    private void DebugFarBackward()
    {
        PlayLoop("Far");
        SetDirectionBackward();
    }

    [Button("👁 Window Pass")]
    private void DebugWindowPass()
    {
        PlayOneShot("WindowPass");
    }

    [Button("⚡ Attack")]
    private void DebugAttack()
    {
        PlayOneShot("Attack");
    }

    [Button("🔄 Reverse")]
    private void DebugReverse()
    {
        ReverseDirection();
    }
}