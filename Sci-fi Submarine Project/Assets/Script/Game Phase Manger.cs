using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class GamePhaseManager : MonoBehaviour
{
    public enum GamePhase
    {
        Calm,
        Suspicion,
        Tension,
        Danger
    }

    [Header("References")]
    [SerializeField] private DepthSystem depthSystem;
    [SerializeField] private LeakMalfunctionManager leakManager;
    [SerializeField] private SystemInterferenceManager systemManager;
    [SerializeField] private MegalodonManager sharkManager;
    [SerializeField] private NodeManager nodeManager;

    [ReadOnly] public GamePhase currentPhase;
    private Coroutine activeRoutine;

    // =========================================================
    // CALM
    // =========================================================
    [Header("Calm Phase")]
    [SerializeField] private float calmStartDelay = 25f;
    [SerializeField] private float calmDuration = 120f;
    [SerializeField] private float calmMinInterval = 10f;
    [SerializeField] private float calmMaxInterval = 20f;
    [SerializeField] private int calmMinMalfunctions = 1;
    [SerializeField] private int calmMaxMalfunctions = 2;

    // =========================================================
    // SUSPICION
    // =========================================================
    [Header("Suspicion Phase")]
    [SerializeField] private float suspicionStartDelay = 25f;
    [SerializeField] private float suspicionDuration = 180f;
    [SerializeField] private float suspicionMinInterval = 8f;
    [SerializeField] private float suspicionMaxInterval = 18f;
    [SerializeField] private int suspicionMinMalfunctions = 1;
    [SerializeField] private int suspicionMaxMalfunctions = 3;

    [Header("Suspicion Shark")]
    [SerializeField] private int sharkEncountersPerSuspicion = 2;
    [SerializeField] private float sharkFirstSpawnDelay = 20f;
    [SerializeField] private float sharkActiveTime = 6f;
    [SerializeField] private float sharkCooldownMin = 25f;
    [SerializeField] private float sharkCooldownMax = 45f;

    [Header("Shark Direction")]
    [SerializeField] private bool startRight = true;
    private int sharkDirectionIndex = 0;

    // =========================================================
    // TENSION
    // =========================================================
    [Header("Tension Phase Timing")]
    [SerializeField] private float tensionDuration = 120f; // 👈 ADD THIS
    [Header("Tension Phase Malfunctions")]
    [SerializeField] private float tensionMalfunctionMinDelay = 8f;
    [SerializeField] private float tensionMalfunctionMaxDelay = 18f;

    [SerializeField] private int tensionMinMalfunctions = 1;
    [SerializeField] private int tensionMaxMalfunctions = 2;

    [SerializeField, Range(0f, 1f)] private float tensionLeakChance = 0.5f;
    [SerializeField, Range(0f, 1f)] private float tensionSystemChance = 0.5f;

    [Header("Tension Shark Phase")]
    [SerializeField] private float tensionStartDelay = 10f;
    [SerializeField] private float tensionFarDuration = 15f;
    [SerializeField] private float tensionSilenceDuration = 10f;

    [SerializeField] private int tensionWindowPassMin = 2;
    [SerializeField] private int tensionWindowPassMax = 3;

    [SerializeField] private float windowPassMinDelay = 10f;
    [SerializeField] private float windowPassMaxDelay = 15f;
    // =========================================================
    // DANGER SETTINGS
    // =========================================================
    [Header("Danger Phase")]
    [SerializeField] private float dangerDuration = 90f;
    [SerializeField] private float dangerStartDelay = 10f;

    [SerializeField] private float dangerMalfunctionMinDelay = 5f;
    [SerializeField] private float dangerMalfunctionMaxDelay = 12f;

    [SerializeField] private int dangerMinMalfunctions = 2;
    [SerializeField] private int dangerMaxMalfunctions = 4;

    [SerializeField, Range(0f, 1f)] private float dangerLeakChance = 0.5f;
    [SerializeField, Range(0f, 1f)] private float dangerSystemChance = 0.5f;

    // =========================================================
    // INIT
    // =========================================================
    void Start()
    {
        SetPhase(GamePhase.Calm);
    }

    void SetPhase(GamePhase newPhase)
    {
        if (currentPhase == newPhase) return;

        currentPhase = newPhase;
        Debug.Log($"📊 Phase → {currentPhase}");

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        sharkDirectionIndex = 0;

        activeRoutine = currentPhase switch
        {
            GamePhase.Calm => StartCoroutine(CalmRoutine()),
            GamePhase.Suspicion => StartCoroutine(SuspicionRoutine()),
            GamePhase.Tension => StartCoroutine(TensionRoutine()),
            GamePhase.Danger => StartCoroutine(DangerRoutine()),
            _ => null
        };
    }


    // =========================================================
    // CALM
    // =========================================================
    IEnumerator CalmRoutine()
    {
        yield return new WaitForSeconds(calmStartDelay);

        float timer = 0f;

        while (timer < calmDuration)
        {
            TriggerCalmMalfunction();

            float wait = Random.Range(calmMinInterval, calmMaxInterval);
            yield return new WaitForSeconds(wait);

            timer += wait;
        }

        SetPhase(GamePhase.Suspicion);
    }

    void TriggerCalmMalfunction()
    {
        int count = Random.Range(calmMinMalfunctions, calmMaxMalfunctions + 1);

        for (int i = 0; i < count; i++)
        {
            if (Random.value < 0.5f)
                leakManager?.TriggerRandomLeak();
            else
                systemManager?.TriggerRandomInterference();
        }
    }

    // =========================================================
    // SUSPICION
    // =========================================================
    IEnumerator SuspicionRoutine()
    {
        yield return new WaitForSeconds(suspicionStartDelay);

        float timer = 0f;

        while (timer < suspicionDuration)
        {
            TriggerSuspicionMalfunction();
            yield return StartCoroutine(SuspicionSharkRoutine());

            yield return new WaitForSeconds(Random.Range(suspicionMinInterval, suspicionMaxInterval));
            timer += suspicionMinInterval;
        }

        SetPhase(GamePhase.Tension);
    }

    void TriggerSuspicionMalfunction()
    {
        int count = Random.Range(suspicionMinMalfunctions, suspicionMaxMalfunctions + 1);

        for (int i = 0; i < count; i++)
        {
            if (Random.value < 0.5f)
                leakManager?.TriggerRandomLeak();
            else
                systemManager?.TriggerRandomInterference();
        }
    }

    IEnumerator SuspicionSharkRoutine()
    {
        yield return new WaitForSeconds(sharkFirstSpawnDelay);

        for (int i = 0; i < sharkEncountersPerSuspicion; i++)
        {
            bool goRight = startRight
                ? (sharkDirectionIndex % 2 == 0)
                : (sharkDirectionIndex % 2 == 1);

            sharkDirectionIndex++;

            if (goRight)
                sharkManager?.SpawnFarRight();
            else
                sharkManager?.SpawnFarLeft();

            yield return new WaitForSeconds(sharkActiveTime);

            sharkManager?.Despawn();

            if (i < sharkEncountersPerSuspicion - 1)
                yield return new WaitForSeconds(Random.Range(sharkCooldownMin, sharkCooldownMax));
        }
    }

    // =========================================================
    // TENSION
    // =========================================================
    IEnumerator TensionRoutine()
{
    yield return new WaitForSeconds(tensionStartDelay);

    Debug.Log("🧠 Tension Phase Started");

    float timer = 0f;

    while (timer < tensionDuration && currentPhase == GamePhase.Tension)
    {
        // =========================
        // MALFUNCTION BEFORE SHARK
        // =========================
        yield return new WaitForSeconds(Random.Range(tensionMalfunctionMinDelay, tensionMalfunctionMaxDelay));
        TriggerTensionMalfunction();

        // =========================
        // FAR APPROACH
        // =========================
        sharkManager?.SpawnFarRight();
        yield return new WaitForSeconds(tensionFarDuration);
        sharkManager?.Despawn();

        // =========================
        // SILENCE
        // =========================
        yield return new WaitForSeconds(tensionSilenceDuration);

        // =========================
        // WINDOW PASSES
        // =========================
        int passCount = Random.Range(tensionWindowPassMin, tensionWindowPassMax + 1);

        for (int i = 0; i < passCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(windowPassMinDelay, windowPassMaxDelay));

            if (nodeManager != null && sharkManager != null)
                ApplyWindowPass(nodeManager.currentNode);
        }

        // =========================
        // SECOND BUILDUP
        // =========================
        yield return new WaitForSeconds(tensionSilenceDuration);

        sharkManager?.SpawnFarLeft();
        yield return new WaitForSeconds(tensionFarDuration);
        sharkManager?.Despawn();

        // =========================
        // FINAL WINDOW PASS
        // =========================
        yield return new WaitForSeconds(Random.Range(windowPassMinDelay, windowPassMaxDelay));

        if (nodeManager != null && sharkManager != null)
            ApplyWindowPass(nodeManager.currentNode);

        // =========================
        // TIME PROGRESSION
        // =========================
        timer += 
            tensionFarDuration +
            tensionSilenceDuration +
            (passCount * windowPassMaxDelay);
    }

    Debug.Log("🧠 Tension Phase Ended");

    // OPTIONAL: loop or transition
    SetPhase(GamePhase.Calm);
}

    void TriggerTensionMalfunction()
    {
        int count = Random.Range(tensionMinMalfunctions, tensionMaxMalfunctions + 1);

        for (int i = 0; i < count; i++)
        {
            if (Random.value < tensionLeakChance)
                leakManager?.TriggerRandomLeak();
            else
                systemManager?.TriggerRandomInterference();
        }
    }

    void ApplyWindowPass(NodeType node)
    {
        switch (node)
        {
            case NodeType.ControlPanel:
                sharkManager.WindowPassFront();
                break;

            case NodeType.Cooling:
            case NodeType.Reactor_Pipes:
                sharkManager.WindowPassHeat();
                break;

            case NodeType.Reactor:
                sharkManager.WindowPassPressure();
                break;

            case NodeType.Electricity:
            case NodeType.Pressure:
                sharkManager.WindowPassPressure();
                break;

            default:
                sharkManager.WindowPassFront();
                break;
        }
    }

     // =========================================================
    // DANGER
    // =========================================================
    // =========================================================
// DANGER
// =========================================================
    IEnumerator DangerRoutine()
    {
        yield return new WaitForSeconds(dangerStartDelay);

        Debug.Log("☠️ Danger Phase Started");

        float timer = 0f;

        while (timer < dangerDuration && currentPhase == GamePhase.Danger)
        {
            float loopStartTime = Time.time;
            // =========================
            // 1. WINDOW PASS (NODE BASED WARNING)
            // =========================
            int passCount = Random.Range(1, 3);

            for (int i = 0; i < passCount; i++)
            {
                yield return new WaitForSeconds(Random.Range(windowPassMinDelay, windowPassMaxDelay));

                if (nodeManager != null && sharkManager != null)
                    ApplyWindowPass(nodeManager.currentNode);
            }

            // =========================
            // 2. CLOSE SPLINE PRESENCE
            // =========================
            Debug.Log("🦈 Danger: Close Approach");

            sharkManager?.SpawnCloseRight(); // or randomize later if you want variation
            yield return new WaitForSeconds(5f); // short aggressive presence

            sharkManager?.SpawnCloseLeft(); // optional second pass
            yield return new WaitForSeconds(5f);

            sharkManager?.Despawn();

            // =========================
            // 3. SILENCE (TENSION BUILDUP)
            // =========================
            yield return new WaitForSeconds(10f);

            // =========================
            // 4. ATTACK
            // =========================
            Debug.Log("💀 DANGER ATTACK");

            if (nodeManager != null && sharkManager != null)
            {
                Debug.Log($"💀 DANGER ATTACK NODE: {nodeManager.currentNode}");
                sharkManager.AttackByNode(nodeManager.currentNode);
            }
            else
            {
                sharkManager?.Attack(); // fallback
            }

            // ensure full attack animation window
            yield return new WaitForSeconds(3f);

            sharkManager?.Despawn();

            // =========================
            // 5. MALFUNCTION SPIKE (OPTIONAL BUT CONSISTENT WITH PHASE)
            // =========================
            TriggerDangerMalfunction();

            // =========================
            // TIME PROGRESSION
            // =========================
            timer += Time.time - loopStartTime;
        }

        Debug.Log("☠️ Danger Phase Ended");

        Debug.Log("☠️ Danger Phase Ended - Game Won");

        // cleanup world first
        ForceEndGameWin();

        // then trigger win screen
        GameStateManager.Instance?.TriggerDemoComplete();
    }

    void TriggerDangerMalfunction()
    {
        int count = Random.Range(dangerMinMalfunctions, dangerMaxMalfunctions + 1);

        for (int i = 0; i < count; i++)
        {
            float roll = Random.value;

            if (roll < dangerLeakChance)
                leakManager?.TriggerRandomLeak();
            else
                systemManager?.TriggerRandomInterference();
        }
    }
    public void ForceEndGameWin()
    {
        StopAllCoroutines();

        currentPhase = GamePhase.Calm;

        // Clear threats
        //leakManager?.StopAllLeaks?.Invoke(); // if you don't have this, see note below
        //systemManager?.StopAllInterference?.Invoke();

        sharkManager?.Despawn();

        Debug.Log("🏁 Game Cleared - Entering Win State");
    }

    // =========================================================
    // DEBUG
    // =========================================================
    [Button] void ForceCalm() => SetPhase(GamePhase.Calm);
    [Button] void ForceSuspicion() => SetPhase(GamePhase.Suspicion);
    [Button] void ForceTension() => SetPhase(GamePhase.Tension);
    [Button] void ForceDanger() => SetPhase(GamePhase.Danger);
    

}