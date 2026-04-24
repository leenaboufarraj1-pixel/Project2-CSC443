using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton to access from anywhere

    [SerializeField] private TextMeshProUGUI scoreText;
    public int currentScore = 0;

    [SerializeField] private int score;
    public int Score => score;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    // Returns the current score value to other scripts (like MenuManager)
    // ... (inside ScoreManager.cs)
    public int GetCurrentScore()
    {
        // FIX: return currentScore, not score
        return currentScore;
    }

    public bool TrySpendScore(int cost)
    {
        if (currentScore >= cost)
        {
            currentScore -= cost;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = $"Points: {currentScore}";
    }
}