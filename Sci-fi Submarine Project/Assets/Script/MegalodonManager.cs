using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class MegalodonManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject megalodonRoot;
    [SerializeField] private MegalodonSplineController splineController;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private LeakMalfunctionManager leakManager;
    [Header("Attack Radar Timing")]
    [SerializeField] private SharkRadarController radarController;
    [SerializeField] private float preAttackDelay = 1.0f;
    [SerializeField] private float preAttackMinDelay = 0.6f;
    [SerializeField] private float preAttackMaxDelay = 1.4f;
    [SerializeField] private bool useRandomPreDelay = false;
    // =========================
    // 🦈 ATTACK VARIANTS
    // =========================

    [Header("Attack A - Pressure Slam")]
    [SerializeField] private float pressureSlamAmount = 40f;
    [SerializeField] private bool pressureSlamTriggersLeak = true;

    [Header("Attack B - Power Disruption")]
    [SerializeField] private float powerDrainAmount = 50f;
    [SerializeField] private float systemLockDuration = 3f;

    [Header("Attack C - Multi Leak Burst")]
    [SerializeField] private int minLeaks = 2;
    [SerializeField] private int maxLeaks = 4;
    [SerializeField] private float leakPressureBonus = 15f;
    

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

        radarController?.SetRadarState(SharkRadarController.RadarState.None);
    }

    // =========================
    // FAR
    // =========================

    public void SpawnFarRight()
    {
        Spawn();
        radarController?.SetRadarState(SharkRadarController.RadarState.Far);
        splineController.PlayFarRight();
    }

    public void SpawnFarLeft()
    {
        Spawn();
        radarController?.SetRadarState(SharkRadarController.RadarState.Far);
        splineController.PlayFarLeft();
    }

    // =========================
    // CLOSE
    // =========================

    public void SpawnCloseRight()
    {
        Spawn();
        radarController?.SetRadarState(SharkRadarController.RadarState.Close);
        splineController.PlayCloseRight();
    }

    public void SpawnCloseLeft()
    {
        Spawn();
        radarController?.SetRadarState(SharkRadarController.RadarState.Close);
        splineController.PlayCloseLeft();
    }

    // =========================
    // WINDOW PASS (DIRECT)
    // =========================

    public void WindowPassRight()
    {
        Spawn();
        radarController?.SetRadarState(SharkRadarController.RadarState.Close);
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassRight();
    }

    public void WindowPassLeft()
    {
        Spawn();
        radarController?.SetRadarState(SharkRadarController.RadarState.Close);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassLeft();
    }

    public void WindowPassFront()
    {
        Spawn();

        radarController?.SetRadarState(SharkRadarController.RadarState.Close);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassFront();
    }

    public void WindowPassPressure()
    {
        Spawn();

        radarController?.SetRadarState(SharkRadarController.RadarState.Close);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySharkWoosh();

        splineController.PlayWindowPassPressure();
    }

    public void WindowPassHeat()
    {
        Spawn();

        radarController?.SetRadarState(SharkRadarController.RadarState.Close);

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
        radarController?.SetRadarState(SharkRadarController.RadarState.Close);
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

        StartCoroutine(PreAttackRadar(() =>
        {
            NodeType node = nodeManager != null ? nodeManager.currentNode : NodeType.ControlPanel;

            ExecuteAttack(node);
        }));
}
    void ExecuteAttack(NodeType node)
    {
        int roll = Random.Range(0, 3);

        // 1️⃣ PLAY ANIMATION BASED ON NODE
        PlayAttackAnimationByNode(node);

        // 2️⃣ APPLY EFFECT BASED ON TYPE
        switch (roll)
        {
            case 0:
                ApplyPressureSlam();
                break;

            case 1:
                ApplyPowerDisruption();
                break;

            case 2:
                ApplyMultiLeak();
                break;
        }
    }
    void ApplyPressureSlam()
    {
        Debug.Log("🦈 ATTACK A: PRESSURE SLAM");

        if (systemManager != null)
            systemManager.pressure += pressureSlamAmount;

        if (pressureSlamTriggersLeak)
            leakManager?.TriggerRandomLeak();
    }
    void ApplyPowerDisruption()
    {
        Debug.Log("🦈 ATTACK B: POWER DISRUPTION");

        if (systemManager != null)
            systemManager.power -= powerDrainAmount;

        StartCoroutine(TemporarySystemLock());
    }
    IEnumerator TemporarySystemLock()
    {
        if (systemManager == null) yield break;

        systemManager.isLocked = true;

        Debug.Log("⚡ Systems Locked");

        yield return new WaitForSeconds(systemLockDuration);

        systemManager.isLocked = false;

        Debug.Log("⚡ Systems Restored");
    }
    void ApplyMultiLeak()
    {
        Debug.Log("🦈 ATTACK C: MULTI LEAK BURST");

        int count = Random.Range(minLeaks, maxLeaks + 1);

        for (int i = 0; i < count; i++)
            leakManager?.TriggerRandomLeak();

        if (systemManager != null)
            systemManager.pressure += leakPressureBonus;
    }
    void PlayAttackAnimationByNode(NodeType node)
    {
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
    IEnumerator PreAttackRadar(System.Action attackAction)
    {
        radarController?.SetRadarState(SharkRadarController.RadarState.Attack);

        float delay = useRandomPreDelay
            ? Random.Range(preAttackMinDelay, preAttackMaxDelay)
            : preAttackDelay;

        yield return new WaitForSeconds(delay);

        attackAction?.Invoke();
    }
    /// <summary>
    /// Helper
    /// </summary>
    private NodeType GetDebugNode()
    {
        if (nodeManager == null)
        {
            nodeManager = FindObjectOfType<NodeManager>();

            if (nodeManager == null)
            {
                Debug.LogWarning("NodeManager is NULL, defaulting to ControlPanel");
                return NodeType.ControlPanel;
            }
        }

        return nodeManager.currentNode;
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
    
    [Button("🦈 FULL Attack (Real)")]
    private void DebugFullAttack()
    {
        Attack();
    }
    [Button("🦈 Test Attack A (Full)")]
    private void DebugAttackA_Full()
    {
        NodeType node = GetDebugNode();
        PlayAttackAnimationByNode(node);
        ApplyPressureSlam();
    }
    [Button("🦈 Test Attack B (Full)")]
    private void DebugAttackB_Full()
    {
        NodeType node = GetDebugNode();
        PlayAttackAnimationByNode(node);
        ApplyPowerDisruption();
    }
    [Button("🦈 Test Attack C (Full)")]
    private void DebugAttackC_Full()
    {
        NodeType node = GetDebugNode();
        PlayAttackAnimationByNode(node);
        ApplyMultiLeak();
    }
}