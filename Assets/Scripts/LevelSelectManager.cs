using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelCard
    {
        public GameObject cardRoot;
        public TextMeshProUGUI levelNameText;
        public TextMeshProUGUI bestTimeText;
        public TextMeshProUGUI pearlCountText;
        public Image medalImage;
        public Image lockOverlay;
        public Button playButton;
    }

    [Header("Level Cards (assign all 5 in order)")]
    public LevelCard[] levelCards; // size 5

    [Header("Medal Sprites")]
    public Sprite bronzeMedal;
    public Sprite silverMedal;
    public Sprite goldMedal;
    public Sprite noMedal;

    [Header("Scene Names (must match Build Settings)")]
    public string[] levelSceneNames; // e.g. "Level1", "Level2" ... size 5

    [Header("Back Button")]
    public Button backButton;
    public string mainMenuSceneName = "MainMenu";

    private string[] levelNames = {
        "Shallow Reef",
        "CoralGarden",
        "Sunken Dhow",
        "Deep Cave",
        "Treasure Cove"
    };

    void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene(mainMenuSceneName));

        for (int i = 0; i < 5; i++)
        {
            int levelIndex = i + 1; // 1-based
            LevelCard card = levelCards[i];
            bool unlocked = GameData.IsUnlocked(levelIndex);

            // Level name
            card.levelNameText.text = "Level " + levelIndex + "\n" + levelNames[i];

            // Lock overlay
            card.lockOverlay.gameObject.SetActive(!unlocked);
            card.playButton.interactable = unlocked;

            if (unlocked)
            {
                // Best time
                float best = GameData.GetBestTime(levelIndex);
                card.bestTimeText.text = best >= 0
                    ? "Best: " + GameData.FormatTime(best)
                    : "Best: --:--";

                // Pearl count
                int pearls = GameData.GetPearls(levelIndex);
                int total = GameData.GetTotalPearls(levelIndex);
                card.pearlCountText.text = total > 0
                    ? "Pearls: " + pearls + "/" + total
                    : "Pearls: -/-";

                // Medal
                int medal = GameData.GetMedal(levelIndex);
                card.medalImage.sprite = medal switch
                {
                    3 => goldMedal,
                    2 => silverMedal,
                    1 => bronzeMedal,
                    _ => noMedal
                };

                // Button loads the level
                card.playButton.onClick.AddListener(() =>
                    SceneManager.LoadScene(levelSceneNames[levelIndex - 1]));
            }
            else
            {
                card.bestTimeText.text = "🔒 Locked";
                card.pearlCountText.text = "";
                card.medalImage.sprite = noMedal;
            }
        }
    }
}