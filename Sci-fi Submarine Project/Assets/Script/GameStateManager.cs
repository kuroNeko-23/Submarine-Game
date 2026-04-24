using UnityEngine;
using UnityEngine.SceneManagement;
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
    }

    void Start()
    {
        SetState(GameState.Playing);

        HideImmediate(gameOverPanel);
        HideImmediate(demoCompletePanel);
    }

    void Update()
    {
        if (currentState != GameState.Playing) return;

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

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAllAudio();

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeInPanel(gameOverPanel));

        Time.timeScale = 0f;
    }

    private void TriggerDemoComplete()
    {
        hasEnded = true;
        SetState(GameState.DemoComplete);

        Debug.Log("🎯 DEMO COMPLETE");

        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAllAudio();

        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeInPanel(demoCompletePanel));

        Time.timeScale = 0f;
    }

    // =========================
    // FADE SYSTEM
    // =========================

    private IEnumerator FadeInPanel(CanvasGroup panel)
    {
        if (panel == null) yield break;

        panel.gameObject.SetActive(true);

        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            float progress = t / fadeDuration;
            panel.alpha = progress;

            yield return null;
        }

        panel.alpha = 1f;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    private void HideImmediate(CanvasGroup panel)
    {
        if (panel == null) return;

        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.gameObject.SetActive(false);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}