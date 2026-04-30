using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private CanvasGroup introCanvasGroup;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;

    [SerializeField] private GameObject creditsPanel;

    [Header("Intro Timing")]
    [SerializeField] private float introFadeInTime = 1.0f;
    [SerializeField] private float introHoldTime = 1.5f;
    [SerializeField] private float introFadeOutTime = 1.0f;
    [SerializeField] private float menuFadeInTime = 1.0f;

    // ---------------- TITLE CARD ----------------

    [Header("Title Card")]
    [SerializeField] private TextMeshProUGUI titleCardText;
    [SerializeField] private CanvasGroup titleCardCanvasGroup;

    [TextArea(2, 4)]
    [SerializeField] private List<string> titleLines = new List<string>()
    {
        "Depth is not empty.",
        "It watches.\nIt waits.",
        "And it will come closer.",
        "Keep the system alive."
    };

    [SerializeField] private float textFadeTime = 0.8f;
    [SerializeField] private float textHoldTime = 1.5f;

    // ---------------- AUDIO ----------------

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    private bool isSkipping = false;
    private bool isShowingTitle = false;
    private bool advanceRequested = false;

    private void Start()
    {
        // Initial states
        introPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);

        introCanvasGroup.alpha = 0f;
        mainMenuCanvasGroup.alpha = 0f;
        titleCardCanvasGroup.alpha = 0f;

        StartCoroutine(IntroFlow());
    }

    private void Update()
    {
        // Skip intro
        if (!isSkipping && !isShowingTitle && Input.anyKeyDown)
        {
            StopAllCoroutines();
            StartCoroutine(SkipToMenu());
        }

        // Title card input (skip / fast-forward)
        if (isShowingTitle && Input.anyKeyDown)
        {
            advanceRequested = true;
        }
    }

    // ---------------- INTRO FLOW ----------------

    private IEnumerator IntroFlow()
    {
        yield return Fade(introCanvasGroup, 0f, 1f, introFadeInTime);
        yield return new WaitForSeconds(introHoldTime);
        yield return Fade(introCanvasGroup, 1f, 0f, introFadeOutTime);

        introPanel.SetActive(false);

        yield return Fade(mainMenuCanvasGroup, 0f, 1f, menuFadeInTime);
    }

    private IEnumerator SkipToMenu()
    {
        isSkipping = true;

        introCanvasGroup.alpha = 0f;
        introPanel.SetActive(false);

        yield return Fade(mainMenuCanvasGroup, 0f, 1f, 0.5f);
    }

    // ---------------- PLAY FLOW ----------------

    public void PlayGame()
    {
        if (!isShowingTitle)
            StartCoroutine(TitleCardSequence());
    }

    private IEnumerator TitleCardSequence()
    {
        isShowingTitle = true;

        // Fade out menu
        yield return Fade(mainMenuCanvasGroup, 1f, 0f, 0.5f);
        mainMenuPanel.SetActive(false);

        titleCardCanvasGroup.alpha = 0f;

        foreach (string line in titleLines)
        {
            advanceRequested = false;
            titleCardText.text = line;

            // Fade in (interruptible)
            yield return Fade(titleCardCanvasGroup, 0f, 1f, textFadeTime);

            // Hold or skip
            float timer = 0f;
            while (timer < textHoldTime)
            {
                if (advanceRequested) break;

                timer += Time.deltaTime;
                yield return null;
            }

            advanceRequested = false;

            // Fade out (interruptible)
            yield return Fade(titleCardCanvasGroup, 1f, 0f, textFadeTime);
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // ---------------- FADE (INTERRUPTIBLE) ----------------

    private IEnumerator Fade(CanvasGroup cg, float start, float end, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            if (advanceRequested) break;

            float t = Mathf.SmoothStep(0f, 1f, time / duration);
            cg.alpha = Mathf.Lerp(start, end, t);

            time += Time.deltaTime;
            yield return null;
        }

        cg.alpha = end;
    }

    // ---------------- UI ----------------

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PlayClick()
    {
        if (clickSound == null || audioSource == null) return;
        audioSource.PlayOneShot(clickSound);
    }
}