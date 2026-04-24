using UnityEngine;
using UnityEngine.Splines;
using Sirenix.OdinInspector;

public class MegalodonSplineController : MonoBehaviour
{
    [Header("Spline References")]
    [SerializeField] private SplineContainer farSpline;
    [SerializeField] private SplineContainer closeSpline;

    [Header("Movement")]
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private bool loop = true;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 2f;

    private SplineContainer currentSpline;
    private SplineContainer targetSpline;

    private float t;

    private bool isTransitioning;
    private float transitionT;

    private SplineContainer fromSpline;

    void Start()
    {
        SetFarSpline();
    }

    void Update()
    {
        if (currentSpline == null) return;

        if (isTransitioning)
        {
            UpdateTransition();
            return;
        }

        MoveAlongSpline();
    }

    // =========================
    // NORMAL MOVEMENT
    // =========================

    private void MoveAlongSpline()
    {
        t += speed * Time.deltaTime;

        if (loop)
            t %= 1f;
        else
            t = Mathf.Clamp01(t);

        transform.position = currentSpline.EvaluatePosition(t);
        transform.rotation = Quaternion.LookRotation(currentSpline.EvaluateTangent(t));
    }

    // =========================
    // SMOOTH TRANSITION
    // =========================

    private void UpdateTransition()
    {
        transitionT += Time.deltaTime / transitionDuration;

        float fromT = t;
        float toT = t;

        Vector3 fromPos = fromSpline.EvaluatePosition(fromT);
        Vector3 toPos = targetSpline.EvaluatePosition(toT);

        Vector3 pos = Vector3.Lerp(fromPos, toPos, transitionT);

        Vector3 fromDir = fromSpline.EvaluateTangent(fromT);
        Vector3 toDir = targetSpline.EvaluateTangent(toT);

        Vector3 dir = Vector3.Slerp(fromDir, toDir, transitionT);

        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(dir);

        if (transitionT >= 1f)
        {
            FinishTransition();
        }
    }

    private void FinishTransition()
    {
        isTransitioning = false;

        currentSpline = targetSpline;
        t = 0f;
    }

    // =========================
    // PUBLIC CONTROL
    // =========================

    public void SetSpline(SplineContainer spline)
    {
        currentSpline = spline;
        t = 0f;
    }

    // 🔥 NEW SMOOTH SWITCH
    public void SwitchSplineSmooth(SplineContainer newSpline)
    {
        if (newSpline == currentSpline) return;

        fromSpline = currentSpline;
        targetSpline = newSpline;

        // 🔥 FIND NEAREST POINT ON NEW SPLINE
        t = FindClosestTOnSpline(targetSpline, transform.position);

        transitionT = 0f;
        isTransitioning = true;
    }
    private float FindClosestTOnSpline(SplineContainer spline, Vector3 worldPos)
    {
        float bestT = 0f;
        float bestDist = float.MaxValue;

        int resolution = 50; // higher = more accurate

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
    // DEBUG BUTTONS
    // =========================

    [Button("➡ Set Far Spline")]
    private void SetFarSpline()
    {
        SetSpline(farSpline);
        Debug.Log("Megalodon switched to FAR spline");
    }

    [Button("⬅ Set Close Spline")]
    private void SetCloseSpline()
    {
        SetSpline(closeSpline);
        Debug.Log("Megalodon switched to CLOSE spline");
    }

    [Button("🌊 Smooth → Far")]
    private void SmoothFar()
    {
        SwitchSplineSmooth(farSpline);
    }

    [Button("🌊 Smooth → Close")]
    private void SmoothClose()
    {
        SwitchSplineSmooth(closeSpline);
    }

    [Button("🔄 Reset Progress")]
    private void ResetSplineProgress()
    {
        t = 0f;
    }
}