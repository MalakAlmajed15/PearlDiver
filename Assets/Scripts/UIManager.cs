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

    [Header("Air Warning Effect")]
    public GameObject vignetteWarning;

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

    [Header("Victory Sound")]
    public AudioClip victorySound;
    public float victorySoundVolume = 1f;

    [Header("Death Sound")]
    public AudioClip deathSound;
    public float deathSoundVolume = 1f;

    private AudioSource audioSource;
    private bool isPaused = false;

    void Awake()
    {
        Instance = this;

        // Set up audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        hudPanel.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (gameActive)
            timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.P)) AddPearl();
        if (Input.GetKeyDown(KeyCode.L)) LoseLife();

        if (Input.GetKeyDown(KeyCode.Escape) && gameActive)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        hudPanel.SetActive(false);
        Time.timeScale = 0f;
        DisablePlayerControls();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        hudPanel.SetActive(true);
        Time.timeScale = 1f;
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

        if (pearls >= totalPearlsInLevel && totalPearlsInLevel > 0)
            ShowVictory();
    }

    public void UpdatePearlUI()
    {
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

        // Play death sound when game over
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound, deathSoundVolume);
            Debug.Log("Death sound played!");
        }

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

        // Play victory sound
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound, victorySoundVolume);
            Debug.Log("Victory sound played!");
        }

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        victoryTimeText.text = string.Format("Duration: {0:00}:{1:00}", minutes, seconds);

        CalculateScore();
        int levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        GameData.SaveLevelResult(levelIndex, timer, pearls, totalPearlsInLevel);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings - 1)
        {
            nextContainer.SetActive(false);
            mainMenuButton.SetActive(true);
            finalScoreText.text = "Grand Total: " + totalScore;
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
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    void CalculateScore()
    {
        int levelScore = (pearls * 100) + (lives * 50);
        int timeBonus = Mathf.Max(0, 500 - Mathf.FloorToInt(timer));
        int totalLevelScore = levelScore + timeBonus;

        if (pearls >= totalPearlsInLevel && totalPearlsInLevel > 0)
        {
            totalScore += totalLevelScore;
            totalPearls += pearls;
        }

        finalScoreText.text = "Level Score: " + totalLevelScore;
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(nextSceneIndex);
            ResetUI();
        }
        else
        {
            nextContainer.SetActive(false);
        }
    }

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
        Time.timeScale = 1f;
        totalScore = 0;
        totalPearls = 0;
        SceneManager.LoadScene(0);
    }

    public void ReturnToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    private void DisablePlayerControls()
    {
        UnderwaterSwimController player = Object.FindFirstObjectByType<UnderwaterSwimController>();
        if (player != null)
        {
            player.enabled = false;
            Animator anim = player.GetComponentInChildren<Animator>();
            if (anim != null) anim.speed = 0f;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EnablePlayerControls()
    {
        UnderwaterSwimController player = Object.FindFirstObjectByType<UnderwaterSwimController>();
        if (player != null)
        {
            player.enabled = true;
            Animator anim = player.GetComponentInChildren<Animator>();
            if (anim != null) anim.speed = 1f;
        }
    }

    public void ToggleLowAirWarning(bool showWarning)
    {
        if (vignetteWarning != null)
        {
            vignetteWarning.SetActive(showWarning);
        }
    }
}