using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Popups")]
    public GameObject instructionsPopup;
    public GameObject creditsPopup;
    public GameObject settingsPopup;

    [Header("Scene Settings")]
    public string levelSceneName = "Shallow Reef";


    public void StartGame()
    {
        SceneManager.LoadScene(levelSceneName); //add the level scene
    }

    public void OpenInstructions()
    {
        instructionsPopup.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPopup.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsPopup.SetActive(true);
    }

    public void ClosePopups()
    {
        instructionsPopup.SetActive(false);
        creditsPopup.SetActive(false);
        settingsPopup.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitting..."); 
        Application.Quit(); 
    }
}