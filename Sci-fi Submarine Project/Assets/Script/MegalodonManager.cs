using UnityEngine;
using Sirenix.OdinInspector;

public class MegalodonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject megalodonRoot;
    [SerializeField] private MegalodonSplineController splineController;
    [SerializeField] private NodeManager nodeManager;

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
    // WINDOW PASS (DIRECT)
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

    public void WindowPassFront()
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassFront();
    }

    public void WindowPassPressure()
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassPressure();
    }

    public void WindowPassHeat()
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassHeat();
    }

    // =========================
    // 🧠 NODE-BASED WINDOW PASS (NEW SYSTEM)
    // =========================

    public void WindowPassByNode(NodeType node)
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        switch (node)
        {
            case NodeType.ControlPanel:
                splineController.PlayWindowPassFront();
                break;

            case NodeType.Cooling:
            case NodeType.Reactor_Pipes:
                splineController.PlayWindowPassHeat();
                break;

            case NodeType.Reactor:
            case NodeType.Pressure:
            case NodeType.Electricity:
                splineController.PlayWindowPassPressure();
                break;

            default:
                splineController.PlayWindowPassFront();
                break;
        }

        // 🧹 FORCE DESPAWN AFTER ACTION
    }
    

    // =========================
    // ATTACK
    // =========================

    public void Attack()
    {
        Spawn();
        splineController.PlayAttack();
    }
    public void AttackByNode(NodeType node)
    {
        Spawn();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        switch (node)
        {
            case NodeType.ControlPanel:
                splineController.PlayAttackFront();
                break;

            case NodeType.Reactor_Pipes:
                splineController.PlayAttackPipes();
                break;

            case NodeType.Cooling:
                splineController.PlayAttackHeat();
                break;

            case NodeType.Reactor:
                splineController.PlayAttackElectricity();
                break;

            case NodeType.Electricity:
                splineController.PlayAttackElectricity();
                break;

            case NodeType.Pressure:
                splineController.PlayAttackPressure();
                break;

            default:
                splineController.PlayAttackFront();
                break;
        }
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

    [Button("🧠 Window (Node Based)")]
    private void DebugWindowNode()
    {
        // safe fallback if NodeManager not referenced here
        var node = FindObjectOfType<NodeManager>();
        if (node != null)
            WindowPassByNode(node.currentNode);
    }

    [Button("🪟 Window Front")]
    private void DebugWindowFront() => WindowPassFront();

    [Button("🪟 Window Pressure")]
    private void DebugWindowPressure() => WindowPassPressure();

    [Button("🪟 Window Heat")]
    private void DebugWindowHeat() => WindowPassHeat();

    [Button("⚡ Attack")]
    private void DebugAttack() => Attack();
    [Button("💀 Attack Front")]
    private void DebugAttackFront()
    {
        Spawn();
        splineController.PlayAttackFront();
    }

    [Button("💀 Attack Heat")]
    private void DebugAttackHeat()
    {
        Spawn();
        splineController.PlayAttackHeat();
    }

    [Button("💀 Attack Pipes")]
    private void DebugAttackPipes()
    {
        Spawn();
        splineController.PlayAttackPipes();
    }

    [Button("💀 Attack Electricity")]
    private void DebugAttackElectricity()
    {
        Spawn();
        splineController.PlayAttackElectricity();
    }

    [Button("💀 Attack Pressure")]
    private void DebugAttackPressure()
    {
        Spawn();
        splineController.PlayAttackPressure();
    }
        [Button("💀 Node Attack Test")]
    private void DebugNodeAttack()
    {
        if (nodeManager == null)
        {
            nodeManager = FindObjectOfType<NodeManager>();

            if (nodeManager == null)
            {
                Debug.LogWarning("NodeManager is NULL");
                return;
            }
        }

        AttackByNode(nodeManager.currentNode);
    }
}