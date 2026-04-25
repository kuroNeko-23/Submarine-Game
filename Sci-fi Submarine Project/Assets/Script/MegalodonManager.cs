using UnityEngine;
using Sirenix.OdinInspector;

public class MegalodonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject megalodonRoot;
    [SerializeField] private MegalodonSplineController splineController;

    private bool isActive;

    // =========================
    // CORE SPAWN SYSTEM
    // =========================

    public void Spawn()
    {
        if (isActive) return;

        megalodonRoot.SetActive(true);
        isActive = true;
    }

    public void Despawn()
    {
        if (!isActive) return;

        megalodonRoot.SetActive(false);
        isActive = false;
    }

    // =========================
    // FAR
    // =========================

    public void SpawnFarRight()
    {
        Spawn();
        splineController.PlayFarRight();
    }

    public void SpawnFarLeft()
    {
        Spawn();
        splineController.PlayFarLeft();
    }

    // =========================
    // CLOSE
    // =========================

    public void SpawnCloseRight()
    {
        Spawn();
        splineController.PlayCloseRight();
    }

    public void SpawnCloseLeft()
    {
        Spawn();
        splineController.PlayCloseLeft();
    }

    // =========================
    // WINDOW PASS
    // =========================

    public void WindowPassRight()
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassRight();
    }

    public void WindowPassLeft()
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassLeft();
    }

    // =========================
    // ATTACK
    // =========================

    public void Attack()
    {
        Spawn();
        splineController.PlayAttack();
    }

    // =========================
    // DEBUG
    // =========================

    [Button("🟢 Spawn")]
    private void DebugSpawn() => Spawn();

    [Button("🔴 Despawn")]
    private void DebugDespawn() => Despawn();

    [Button("🌊 Far Right")]
    private void DebugFarRight() => SpawnFarRight();

    [Button("🌊 Far Left")]
    private void DebugFarLeft() => SpawnFarLeft();

    [Button("🪸 Close Right")]
    private void DebugCloseRight() => SpawnCloseRight();

    [Button("🪸 Close Left")]
    private void DebugCloseLeft() => SpawnCloseLeft();

    [Button("🪟 Window Right")]
    private void DebugWindowRight() => WindowPassRight();

    [Button("🪟 Window Left")]
    private void DebugWindowLeft() => WindowPassLeft();

    [Button("⚡ Attack")]
    private void DebugAttack() => Attack();
}