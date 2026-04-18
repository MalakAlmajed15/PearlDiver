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
    public LevelCard[] levelCards;

    [Header("Medal Sprites")]
    public Sprite bronzeMedal;
    public Sprite silverMedal;
    public Sprite goldMedal;

    [Header("Scene Names (must match Build Settings)")]
    public string[] levelSceneNames;

    [Header("Back Button")]
    public Button backButton;
    public string mainMenuSceneName = "MainMenu";

    private string[] levelNames = {
        "Shallow Reef",
        "Coral Garden",
        "Sunken Dhow",
        "Deep Cave",
        "Treasure Cove"
    };

    void Start()
    {
        backButton.onClick.AddListener(() => SceneManager.LoadScene(mainMenuSceneName));

        for (int i = 0; i < 5; i++)
        {
            int levelIndex = i + 1;
            LevelCard card = levelCards[i];
            bool unlocked = GameData.IsUnlocked(levelIndex);

            //card.levelNameText.text = "Level " + levelIndex + "\n" + levelNames[i];

            if (card.lockOverlay != null)
                card.lockOverlay.gameObject.SetActive(!unlocked);

            card.playButton.interactable = unlocked;

            if (unlocked)
            {
                float best = GameData.GetBestTime(levelIndex);
                card.bestTimeText.text = best >= 0
                    ? "Best: " + GameData.FormatTime(best)
                    : "Best: --:--";

                int pearls = GameData.GetPearls(levelIndex);
                int total = GameData.GetTotalPearls(levelIndex);
                card.pearlCountText.text = total > 0
                    ? "Pearls: " + pearls + "/" + total
                    : "Pearls: -/-";

                // Show medal only if earned, hide otherwise
                int medal = GameData.GetMedal(levelIndex);
                if (medal == 0)
                {
                    card.medalImage.gameObject.SetActive(false);
                }
                else
                {
                    card.medalImage.gameObject.SetActive(true);
                    card.medalImage.sprite = medal switch
                    {
                        3 => goldMedal,
                        2 => silverMedal,
                        _ => bronzeMedal
                    };
                }

                string sceneToLoad = levelSceneNames[i];
                card.playButton.onClick.AddListener(() =>
                    SceneManager.LoadScene(sceneToLoad));
            }
            else
            {
                card.bestTimeText.text = "Play levels to unlock";
                card.pearlCountText.text = "";
                card.medalImage.gameObject.SetActive(false); 
            }
        }
    }
}