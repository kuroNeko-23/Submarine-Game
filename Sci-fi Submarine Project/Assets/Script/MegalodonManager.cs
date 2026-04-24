using UnityEngine;
using Sirenix.OdinInspector;

public class MegalodonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject megalodonRoot;
    [SerializeField] private MegalodonSplineController splineController;

    private bool isActive = false;

    // =========================
    // CORE CONTROL
    // =========================

    public void Spawn()
    {
        if (isActive) return;

        megalodonRoot.SetActive(true);
        isActive = true;

        Debug.Log("🦈 Megalodon Spawned");
    }

    public void Despawn()
    {
        if (!isActive) return;

        megalodonRoot.SetActive(false);
        isActive = false;

        Debug.Log("👻 Megalodon Despawned");
    }

    // =========================
    // BEHAVIOR
    // =========================

    public void SpawnFar()
    {
        Spawn();
        splineController.PlayLoop("Far");
    }

    public void SpawnClose()
    {
        Spawn();
        splineController.PlayLoop("Close");
    }

    public void Attack()
    {
        Spawn();
        splineController.PlayOneShot("Attack");
    }
    public void WindowPass()
    {
        Spawn(); // ensure it's active

        splineController.PlayOneShot("WindowPass");

        Debug.Log("🪟 Megalodon Window Pass");
    }

    // =========================
    // DEBUG
    // =========================

    [Button("🟢 Spawn")]
    private void DebugSpawn()
    {
        Spawn();
    }

    [Button("🔴 Despawn")]
    private void DebugDespawn()
    {
        Despawn();
    }

    [Button("🌊 Spawn Far")]
    private void DebugFar()
    {
        SpawnFar();
    }

    [Button("🪟 Spawn Close")]
    private void DebugClose()
    {
        SpawnClose();
    }

    [Button("⚡ Attack")]
    private void DebugAttack()
    {
        Attack();
    }
    [Button("🪟 Window Pass")]
    private void DebugWindowPass()
    {
        WindowPass();
    }
}