using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        // Default state
        ShowMainMenu();
    }

    // ---------------- MAIN BUTTONS ----------------

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        // Works in build only
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ---------------- CREDITS BUTTON ----------------

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // ---------------- HELPERS ----------------

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }
     public void PlayClick()
    {
        if (clickSound == null || audioSource == null) return;

        audioSource.PlayOneShot(clickSound);
    }
}