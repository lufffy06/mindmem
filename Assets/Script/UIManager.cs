using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text highScoreText;
    public Text pairsText;

    private void Start()
    {
        // Initialize after GameManager is ready
        InitializeUI();
    }

    private void OnEnable()
    {
        // If GameManager exists, initialize UI
        if (GameManager.Instance != null)
        {
            InitializeUI();
        }
    }

    private void InitializeUI()
    {
        // Safe initialization
        if (GameManager.Instance == null) return;

        // Subscribe to events
        GameManager.Instance.OnScoreUpdated += UpdateScore;
        GameManager.Instance.OnHighScoreUpdated += UpdateHighScore;
        GameManager.Instance.OnPairsUpdated += UpdatePairs;

        // Initialize UI with current values
        UpdateScore(GameManager.Instance.GetHighScore());
        UpdateHighScore(GameManager.Instance.GetHighScore());
        UpdatePairs(0, 0);
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnScoreUpdated -= UpdateScore;
        GameManager.Instance.OnHighScoreUpdated -= UpdateHighScore;
        GameManager.Instance.OnPairsUpdated -= UpdatePairs;
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }

    private void UpdatePairs(int matched, int total)
    {
        if (pairsText != null)
            pairsText.text = "Pairs: " + matched + "/" + total;
    }
}