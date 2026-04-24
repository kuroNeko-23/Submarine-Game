using UnityEngine;
using UnityEngine.Splines;

public class MegalodonSplineMotor : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private SplineContainer spline;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 0.1f;

    private float speedMultiplier = 1f;
    private bool loop = true;

    private float t;

    void Update()
    {
        if (spline == null) return;

        Move();
    }

    private void Move()
    {
        t += baseSpeed * speedMultiplier * Time.deltaTime;

        if (loop)
            t %= 1f;
        else
            t = Mathf.Clamp01(t);

        transform.position = spline.EvaluatePosition(t);
        transform.rotation = Quaternion.LookRotation(spline.EvaluateTangent(t));
    }

    // =========================
    // PUBLIC CONTROL API
    // =========================

    public void SetSpline(SplineContainer newSpline)
    {
        spline = newSpline;
        t = 0f;
    }

    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = value;
    }

    public void SetLoop(bool value)
    {
        loop = value;
    }
}