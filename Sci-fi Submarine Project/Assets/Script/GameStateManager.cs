using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        DemoComplete
    }

    [Header("References")]
    [SerializeField] private SystemManager systemManager;
    [SerializeField] private DepthSystem depthSystem;

    [Header("UI Panels (CanvasGroup REQUIRED)")]
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private CanvasGroup demoCompletePanel;

    [Header("Depth Settings")]
    [SerializeField] private float targetDepth = 1000f;

    [Header("UI Fade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [Header("Auto Restart")]
    [SerializeField] private float autoRestartDelay = 5f;
    [SerializeField] private string gameplaySceneName;

    private GameState currentState;
    private bool hasEnded = false;

    private Coroutine currentFade;

    // =========================
    // INIT
    // =========================

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1f; // ✅ FORCE NORMAL TIME ON START

        hasEnded = false;     // ✅ RESET STATE SAFELY

        if (EventSystem.current == null)
        {
            Debug.LogError("NO EVENT SYSTEM FOUND IN SCENE!");
        }
    }

    void Start()
    {
        SetState(GameState.Playing);

        HideImmediate(gameOverPanel);
        HideImmediate(demoCompletePanel);

        SetCursorAlwaysOn(); // ✅ ADD THIS
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

        SetCursorAlwaysOn(); // ✅ ensures nothing overrides it

        CheckLoseCondition();
        CheckDemoComplete();
    }

    // =========================
    // CONDITIONS
    // =========================

    private void CheckLoseCondition()
    {
        if (hasEnded || systemManager == null) return;

        if (systemManager.integrity <= 0f)
        {
            TriggerGameOver();
        }
    }
    private void SetCursorAlwaysOn()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CheckDemoComplete()
    {
        if (hasEnded || depthSystem == null) return;

        if (depthSystem.CurrentDepth >= targetDepth)
        {
            TriggerDemoComplete();
        }
    }

    // =========================
    // STATE CHANGES
    // =========================

    private void TriggerGameOver()
{
    hasEnded = true;
    SetState(GameState.GameOver);

    Debug.Log("💀 GAME OVER");

    SetCursorAlwaysOn();

    if (AudioManager.Instance != null)
        AudioManager.Instance.FadeOutAllAudio();

    if (currentFade != null) StopCoroutine(currentFade);
    currentFade = StartCoroutine(FadeInPanel(gameOverPanel));

    Time.timeScale = 0f;

    StartCoroutine(AutoRestartRoutine()); // ✅ ADD THIS
}
public void TriggerDemoComplete()
{
    hasEnded = true;
    SetState(GameState.DemoComplete);

    Debug.Log("🎯 DEMO COMPLETE");

    SetCursorAlwaysOn();

    if (AudioManager.Instance != null)
        AudioManager.Instance.FadeOutAllAudio();

    if (currentFade != null) StopCoroutine(currentFade);
    currentFade = StartCoroutine(FadeInPanel(demoCompletePanel));

    Time.timeScale = 0f;

    StartCoroutine(AutoRestartRoutine()); // ✅ ADD THIS
}

    // =========================
    // FADE SYSTEM
    // =========================

    private IEnumerator FadeInPanel(CanvasGroup panel)
    {
        if (panel == null) yield break;

        panel.gameObject.SetActive(true);

        panel.alpha = 0f;
        panel.interactable = true;      // 👈 MOVE EARLY
        panel.blocksRaycasts = true;    // 👈 MOVE EARLY

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            panel.alpha = t / fadeDuration;
            yield return null;
        }

        panel.alpha = 1f;
    }

    private void HideImmediate(CanvasGroup panel)
    {
        if (panel == null) return;

        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.gameObject.SetActive(false);
    }
    private IEnumerator AutoRestartRoutine()
    {
        yield return new WaitForSecondsRealtime(autoRestartDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // STATE
    // =========================

    private void SetState(GameState newState)
    {
        currentState = newState;
    }

    // =========================
    // BUTTONS
    // =========================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (currentFade != null)
            StopCoroutine(currentFade);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if(AudioManager.Instance != null)
            AudioManager.Instance.FadeInAllAudio();
    }
    

    public void GoToMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void StartGame(string gameplaySceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }
    
    // =========================
    // DEBUG BUTTONS
    // =========================

    [Button("💀 Force Game Over (Debug)")]
    private void DebugGameOver()
    {
        TriggerGameOver();
    }

    [Button("🎯 Force Win (Debug)")]
    private void DebugWin()
    {
        TriggerDemoComplete();
    }

    [Button("▶ Reset Game State")]
    private void DebugResetState()
    {
        Time.timeScale = 1f;
        hasEnded = false;
        SetState(GameState.Playing);

        HideImmediate(gameOverPanel);
        HideImmediate(demoCompletePanel);

        Debug.Log("🔄 Game State Reset");
    }
}