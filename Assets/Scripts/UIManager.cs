using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public int pearls = 0;
    public int lives = 3;
    private float timer = 0f;
    private bool gameActive = true;
    public int totalScore = 0; 
    public int totalPearls = 0;

    public TextMeshProUGUI pearlText;
    public Image[] heartImages;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject hudPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI victoryTimeText;
    public GameObject nextButton;
    public GameObject mainMenuButton;
    public GameObject nextContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            timer += Time.deltaTime; // Track elapsed time
        }
        // DEBUG: until the level is built
        if (Input.GetKeyDown(KeyCode.P)) AddPearl();
        if (Input.GetKeyDown(KeyCode.L)) LoseLife();

    }

    public void AddPearl()
    {
        pearls++;
        pearlText.text = pearls + "/10" ;
        if (pearls >= 10) ShowVictory();
    }

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            heartImages[lives].enabled = false; // Hides the heart
        }

        if (lives <= 0) ShowGameOver();
    }

    void ShowGameOver()
    {
        gameActive = false;
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        CalculateScore();
    }

    void ShowVictory()
    {
        gameActive = false;
        hudPanel.SetActive(false);
        victoryPanel.SetActive(true);
        
        // Calculate time for dispaly
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        victoryTimeText.text = string.Format("Duration: {0:00}:{1:00}", minutes, seconds);
        
        CalculateScore();

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Check if this is the final level
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings) 
        {
            nextContainer.SetActive(false);
            mainMenuButton.SetActive(true);
            finalScoreText.text = "Grand Total: " + totalScore ;
        }
        else
        {
            nextButton.SetActive(true);
            mainMenuButton.SetActive(true);
        }


    }

    public void RestartLevel()
    {
        ResetUI();

        // Reload the game scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }


    void CalculateScore()
    {
        // 100 points per pearl + bonus for staying alive + time bonus (500 points minus the seconds elapsed)
        // Calculate one level score
        int levelScore = (pearls * 100) + (lives * 50);
        int timeBonus = Mathf.Max(0, 500 - Mathf.FloorToInt(timer));
        int totalLevelScore = levelScore + timeBonus;

        // Update CUMULATIVE stats (only if winning)
        if (pearls >= 10)
        {
            totalScore += totalLevelScore;
            totalPearls += pearls;
        }

        // Dispaly
        finalScoreText.text = "Level Score: " + totalLevelScore;
    }

    // Add this for the "Next" button
    public void LoadNextLevel()
    {
        // Get the index of the current level and add 1
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if the next scene actually exists in Build Settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            ResetUI(); // Reset stats for the new level
        }
        else
        {
            nextContainer.SetActive(false);
        }
    }

    // Helper to clean up stats when moving between levels or restarting
    void ResetUI()
    {
        pearls = 0;
        lives = 3;
        timer = 0;
        gameActive = true;
        pearlText.text = "0/10";
        foreach (Image img in heartImages) img.enabled = true;

        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        // Reset everything before leaving
        totalScore = 0;
        totalPearls = 0;
        SceneManager.LoadScene(0); // Load scene at index 0 (Main Menu)
    }
}
