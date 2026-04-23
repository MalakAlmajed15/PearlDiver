using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInitializer : MonoBehaviour
{
    public string uiSceneName = "UI Scene";

    void Awake()
    {
        // Check if UI scene is already loaded
        bool isLoaded = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == uiSceneName)
            {
                isLoaded = true;
                break;
            }
        }

        if (!isLoaded)
        {
            SceneManager.LoadScene(uiSceneName, LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        // 2. Count the pearls in the CURRENT scene
        // We look for any object with the PearlManager script
        PearlManager[] pearlsInScene = Object.FindObjectsByType<PearlManager>(FindObjectsSortMode.None);
        int count = pearlsInScene.Length;

        // 3. Send that count to the UIManager
        SetTotalPearls(count);
    }

    private void SetTotalPearls(int count)
    {
        // If UI loaded slowly, we might need to find it manually
        if (UIManager.Instance == null)
        {
            UIManager.Instance = Object.FindFirstObjectByType<UIManager>();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.totalPearlsInLevel = count;
            UIManager.Instance.UpdatePearlUI();
            Debug.Log("Level Initialized: Found " + count + " pearls.");
        }
    }
}
