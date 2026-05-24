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
    public int totalPearlsInLevel = 0;

    [Header("UI Panels")]
    public GameObject hudPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject pausePanel; 

    [Header("Text & Images")]
    public TextMeshProUGUI pearlText;
    public Image[] heartImages;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI victoryTimeText;

    [Header("Buttons")]
    public GameObject nextButton;
    public GameObject mainMenuButton;
    public GameObject backToLevelSelect;
    public GameObject nextContainer;

    private bool isPaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        hudPanel.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
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

        if (Input.GetKeyDown(KeyCode.Escape) && gameActive)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        hudPanel.SetActive(false); 
        Time.timeScale = 0f; // Freezes the game world and timer
        DisablePlayerControls();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        hudPanel.SetActive(true); 
        Time.timeScale = 1f; // Unfreezes the game world
        EnablePlayerControls();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void AddPearl()
    {
        pearls++;
        UpdatePearlUI();

        // Victory is now based on the actual number of pearls in the scene
        if (pearls >= totalPearlsInLevel && totalPearlsInLevel > 0)
        {
            ShowVictory();
        }
    }

    public void UpdatePearlUI()
    {
        // Now it shows "1/5", "2/12", etc., based on the level
        pearlText.text = pearls + "/" + totalPearlsInLevel;
    }

    public void LoseLife()
    {
        if (lives > 0)
        {
            lives--;
            heartImages[lives].enabled = false;
        }

        if (lives <= 0) ShowGameOver();
    }

    public void AddLife()
    {
      
        if (lives < heartImages.Length)
        {
            heartImages[lives].enabled = true;
            lives++;
            Debug.Log("Extra life! Lives: " + lives);
        }
        else
        {
            Debug.Log("Already at max lives!");
        }
    }

    void ShowGameOver()
    {
        gameActive = false;
        Time.timeScale = 0f;
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        DisablePlayerControls();
        CalculateScore();

        int levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        GameData.SaveLevelResult(levelIndex, timer, pearls, totalPearlsInLevel);
    }

    void ShowVictory()
    {
        gameActive = false;
        Time.timeScale = 0f;
        hudPanel.SetActive(false);
        victoryPanel.SetActive(true);
        DisablePlayerControls();

        // Calculate time for dispaly
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        victoryTimeText.text = string.Format("Duration: {0:00}:{1:00}", minutes, seconds);
        
        CalculateScore();
        int levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        GameData.SaveLevelResult(levelIndex, timer, pearls, totalPearlsInLevel);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Check if this is the final level
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings - 1) 
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
        if (pearls >= totalPearlsInLevel && totalPearlsInLevel > 0)
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
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
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
        EnablePlayerControls();
        Time.timeScale = 1f;
        pearls = 0;
        lives = 3;
        timer = 0;
        gameActive = true;
        pearlText.text = "0/10";
        foreach (Image img in heartImages) img.enabled = true;

        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        pausePanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        // Reset everything before leaving
        Time.timeScale = 1f;
        totalScore = 0;
        totalPearls = 0;
        SceneManager.LoadScene(0); // Load scene at index 0 (Main Menu)
    }


    public void ReturnToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    // HELPER FUNCTIONS
    private void DisablePlayerControls()
    {
        // 1. Freeze the player and animations
        UnderwaterSwimController player = Object.FindFirstObjectByType<UnderwaterSwimController>();
        if (player != null)
        {
            player.enabled = false;
            Animator anim = player.GetComponentInChildren<Animator>();
            if (anim != null) anim.speed = 0f;
        }

        // 2. Force the cursor to appear and unlock so we can click UI buttons
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EnablePlayerControls()
    {
        // Unfreeze the player and animations
        UnderwaterSwimController player = Object.FindFirstObjectByType<UnderwaterSwimController>();
        if (player != null)
        {
            player.enabled = true;
            Animator anim = player.GetComponentInChildren<Animator>();
            if (anim != null) anim.speed = 1f;
        }
    }
}
