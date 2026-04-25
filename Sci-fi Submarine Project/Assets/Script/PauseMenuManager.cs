using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        ResumeGame(); // Ensure game starts unpaused
    }

    private void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ---------------- PAUSE ----------------

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Optional: unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ---------------- RESUME ----------------

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Optional: lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ---------------- EXIT ----------------

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // IMPORTANT: reset time before switching scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}