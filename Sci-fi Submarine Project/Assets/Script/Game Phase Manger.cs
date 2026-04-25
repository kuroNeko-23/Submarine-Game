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

    private int sharkDirectionIndex = 0;
    [SerializeField] private bool startRight = true;

    // =========================================================
    // TENSION
    // =========================================================
    [Header("Tension Phase")]
    [SerializeField] private float tensionDuration = 120f;
    [SerializeField] private float tensionStartDelay = 10f;
    [SerializeField] private float tensionMalfunctionMinDelay = 8f;
    [SerializeField] private float tensionMalfunctionMaxDelay = 18f;
    [SerializeField] private int tensionMinMalfunctions = 1;
    [SerializeField] private int tensionMaxMalfunctions = 2;
    [SerializeField, Range(0f, 1f)] private float tensionLeakChance = 0.5f;

    [Header("Tension Shark")]
    [SerializeField] private float tensionFarDuration = 15f;
    [SerializeField] private float tensionSilenceDuration = 10f;

    // =========================================================
    // DANGER
    // =========================================================
    [Header("Danger Phase")]
    [SerializeField] private float dangerDuration = 90f;
    [SerializeField] private float dangerStartDelay = 10f;
    [SerializeField, Range(0f, 1f)] private float dangerLeakChance = 0.5f;
    [SerializeField] private int dangerMinMalfunctions = 2;
    [SerializeField] private int dangerMaxMalfunctions = 4;

    // =========================================================
    // INIT
    // =========================================================
    void Start()
    {
        StartCoroutine(GameLoop());
    }

    // =========================================================
    // MASTER LOOP
    // =========================================================
    IEnumerator GameLoop()
    {
        while (true)
        {
            currentPhase = GamePhase.Calm;
            yield return StartCoroutine(CalmRoutine());

            currentPhase = GamePhase.Suspicion;
            yield return StartCoroutine(SuspicionRoutine());

            currentPhase = GamePhase.Tension;
            yield return StartCoroutine(TensionRoutine());

            currentPhase = GamePhase.Danger;
            yield return StartCoroutine(DangerRoutine());

            yield break; // stop after win
        }
    }

    // =========================================================
    // CALM
    // =========================================================
    IEnumerator CalmRoutine()
    {
        yield return new WaitForSeconds(calmStartDelay);
        Debug.Log("🌊 Entering Calm Phase");
        float endTime = Time.time + calmDuration;

        while (Time.time < endTime)
        {
            TriggerCalmMalfunction();
            yield return new WaitForSeconds(Random.Range(calmMinInterval, calmMaxInterval));
        }
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
        Debug.Log("👀 Entering Suspicion Phase");
        float endTime = Time.time + suspicionDuration;

        StartCoroutine(SuspicionSharkRoutine());

        while (Time.time < endTime)
        {
            TriggerSuspicionMalfunction();
            yield return new WaitForSeconds(Random.Range(suspicionMinInterval, suspicionMaxInterval));
        }
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
        Debug.Log("😰 Entering Tension Phase");
        float endTime = Time.time + tensionDuration;

        StartCoroutine(TensionSharkLoop());

        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(
                Random.Range(tensionMalfunctionMinDelay, tensionMalfunctionMaxDelay)
            );

            TriggerTensionMalfunction();

            yield return new WaitForSeconds(tensionSilenceDuration);
        }
    }

    IEnumerator TensionSharkLoop()
    {
        yield return new WaitForSeconds(tensionStartDelay);

        while (currentPhase == GamePhase.Tension)
        {
            // =========================
            // FAR APPROACH RIGHT
            // =========================
            sharkManager?.SpawnFarRight();
            yield return new WaitForSeconds(tensionFarDuration);
            sharkManager?.Despawn();

            // =========================
            // SILENCE
            // =========================
            yield return new WaitForSeconds(tensionSilenceDuration);

            // =========================
            // 🔥 WINDOW PASSES (ADDED)
            // =========================
            int passCount = Random.Range(2, 4);

            for (int i = 0; i < passCount; i++)
            {
                yield return new WaitForSeconds(Random.Range(3f, 6f));

                if (nodeManager != null && sharkManager != null)
                {
                    ApplyWindowPass(nodeManager.currentNode);
                }
            }

            // =========================
            // FAR APPROACH LEFT
            // =========================
            sharkManager?.SpawnFarLeft();
            yield return new WaitForSeconds(tensionFarDuration);
            sharkManager?.Despawn();

            // =========================
            // FINAL WINDOW PASS
            // =========================
            yield return new WaitForSeconds(Random.Range(4f, 8f));

            if (nodeManager != null && sharkManager != null)
            {
                ApplyWindowPass(nodeManager.currentNode);
            }

            // =========================
            // SILENCE LOOP
            // =========================
            yield return new WaitForSeconds(tensionSilenceDuration);
        }
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

    // =========================================================
    // DANGER
    // =========================================================
    IEnumerator DangerRoutine()
    {
        yield return new WaitForSeconds(dangerStartDelay);
        Debug.Log("😨 Entering Danger Phase");

        float endTime = Time.time + dangerDuration;

        while (Time.time < endTime)
        {
            int passCount = Random.Range(1, 3);

            for (int i = 0; i < passCount; i++)
            {
                yield return new WaitForSeconds(Random.Range(5f, 12f));
                ApplyWindowPass(nodeManager.currentNode);
            }

            sharkManager?.SpawnCloseRight();
            yield return new WaitForSeconds(5f);

            sharkManager?.SpawnCloseLeft();
            yield return new WaitForSeconds(5f);

            sharkManager?.Despawn();

            yield return new WaitForSeconds(10f);

            sharkManager?.Attack();
            yield return new WaitForSeconds(3f);

            sharkManager?.Despawn();

            TriggerDangerMalfunction();
        }

        ForceEndGameWin();
        GameStateManager.Instance?.TriggerDemoComplete();
    }

    void TriggerDangerMalfunction()
    {
        int count = Random.Range(dangerMinMalfunctions, dangerMaxMalfunctions + 1);

        for (int i = 0; i < count; i++)
        {
            if (Random.value < dangerLeakChance)
                leakManager?.TriggerRandomLeak();
            else
                systemManager?.TriggerRandomInterference();
        }
    }

    void ApplyWindowPass(NodeType node)
    {
        switch (node)
        {
            case NodeType.Cooling:
            case NodeType.Reactor_Pipes:
                sharkManager.WindowPassHeat();
                break;

            case NodeType.Reactor:
            case NodeType.Pressure:
            case NodeType.Electricity:
                sharkManager.WindowPassPressure();
                break;

            default:
                sharkManager.WindowPassFront();
                break;
        }
    }

    public void ForceEndGameWin()
    {
        StopAllCoroutines();
        sharkManager?.Despawn();
        Debug.Log("🏁 Game Complete - Win State");
    }
}