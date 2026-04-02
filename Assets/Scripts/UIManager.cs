using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public int pearls = 0;
    public int lives = 3;

    public TextMeshProUGUI pearlText;
    public Image[] heartImages;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject hudPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG: until the level is built
        if (Input.GetKeyDown(KeyCode.P)) AddPearl();
        if (Input.GetKeyDown(KeyCode.L)) LoseLife();

    }

    public void AddPearl()
    {
        pearls++;
        pearlText.text = "10/" + pearls;
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
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    void ShowVictory()
    {
        hudPanel.SetActive(false);
        victoryPanel.SetActive(true);
    }
}
