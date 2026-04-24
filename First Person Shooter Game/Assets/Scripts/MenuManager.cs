using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Standard practice to include this for TextMeshPro

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject howToPlayPanel;

    [Header("End Game UI")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText; // Ensure this type is used
    [SerializeField] private GameObject GameOverPanel;

    public void StartGame()
    {
        // Reset time in case we are coming from a Victory/GameOver screen
        SceneManager.LoadScene(1);

        // Hide cursor for FPS gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToMenu()
    {
        // Critical: Reset time so the menu isn't slow-mo
        Time.timeScale = 1f;

        // Make cursor visible for menu navigation
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(0);

        PlayClickSound();
    }

    public void RestartGame()
    {
        // Reset time and reload the game level
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);

        // Relock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayClickSound();
    }

    public void ToggleHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            bool isActive = howToPlayPanel.activeSelf;
            howToPlayPanel.SetActive(!isActive);
            PlayClickSound();
        }
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            PlayClickSound();
        }
    }

    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            // Unlock cursor for clicking buttons
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (ScoreManager.Instance != null && finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + ScoreManager.Instance.GetCurrentScore().ToString();
            }

            Time.timeScale = 0.2f;
        }
    }

    // Helper method to keep the code clean
    private void PlayClickSound()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.uiClickSound != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.uiClickSound);
        }
    }

    public void ShowGameOver()
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);

            // Unlock cursor so they can click "Restart" or "Quit"
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // Freeze time so animals stop attacking while the screen is up
            Time.timeScale = 0f;

            if (ScoreManager.Instance != null && finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + ScoreManager.Instance.GetCurrentScore().ToString();
            }
        }
    }
}