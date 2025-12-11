using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public Button continueButton;
    public Button easyButton;
    public Button mediumButton;
    public Button hardButton;

    [Header("Scene References")]
    public GameObject difficultySelectionPanel;
    public GameObject mainButtonsPanel;

    private void Start()
    {
        // Initialize button states
        continueButton.gameObject.SetActive(GameManager.Instance.HasSavedGame);

        // Set up button listeners
        continueButton.onClick.AddListener(OnContinueClicked);
        easyButton.onClick.AddListener(() => OnNewGameClicked(Difficulty.Easy));
        mediumButton.onClick.AddListener(() => OnNewGameClicked(Difficulty.Medium));
        hardButton.onClick.AddListener(() => OnNewGameClicked(Difficulty.Hard));

        // Show appropriate panel
        difficultySelectionPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }
    // Attach this to your "New Game" button in the Inspector
    public void OnNewGameButtonClicked()
    {
        // This shows the difficulty selection panel
        ShowDifficultySelection();
    }

    // Then when a difficulty is selected (attached to Easy/Medium/Hard buttons)
    public void OnDifficultySelected(int difficulty)
    {
        GameManager.Instance.StartNewGame((Difficulty)difficulty);
    }

    public void ShowDifficultySelection()
    {
        mainButtonsPanel.SetActive(false);
        difficultySelectionPanel.SetActive(true);
    }

    public void OnNewGameClicked(Difficulty difficulty)
    {
        GameManager.Instance.StartNewGame(difficulty);
        // Optional: Load your game scene here if using multiple scenes
        // SceneManager.LoadScene("GameScene");
    }

    public void OnContinueClicked()
    {
        GameManager.Instance.ResumeGame();
        // Optional: Load your game scene here if using multiple scenes
        // SceneManager.LoadScene("GameScene");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}