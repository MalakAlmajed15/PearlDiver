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
        public Button playButton;
    }

    [Header("Level Cards (assign all 4 in order)")]
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

    [Header("Button Style")]
    public Color lockedButtonColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color buttonTextColor = Color.white;

    // Level 1 is always unlocked; subsequent levels unlock when the previous is completed.
    // "Completed" means GameData.IsUnlocked(nextLevelIndex), which your GameData should
    // set to true when the player finishes a level.
    private readonly string[] levelNames = {
        "Shallow Reef",
        "Coral Garden",
        "Deep Cave",
        "Treasure Cove"
    };

    void Start()
    {
        backButton.onClick.AddListener(() =>
            SceneManager.LoadScene(mainMenuSceneName));

        for (int i = 0; i < 4; i++)
        {
            int levelIndex = i + 1;       // 1-based level index
            LevelCard card = levelCards[i];

            // Level 1 is always unlocked.
            // Level N is unlocked only if level N-1 has been completed.
            // GameData.IsUnlocked(levelIndex) should return true once that level is unlocked.
            bool unlocked = (levelIndex == 1) || GameData.IsUnlocked(levelIndex);

            // Set the level name on the card
            if (card.levelNameText != null)
                card.levelNameText.text = levelNames[i];

            card.playButton.interactable = unlocked;
            StyleButton(card.playButton, unlocked);

            if (unlocked)
            {
                // --- Best Time ---
                float best = GameData.GetBestTime(levelIndex);
                card.bestTimeText.text = best >= 0
                    ? "Best: " + GameData.FormatTime(best)
                    : "Best: --:--";

                // --- Pearl Count ---
                int pearls = GameData.GetPearls(levelIndex);
                int total = GameData.GetTotalPearls(levelIndex);
                card.pearlCountText.text = total > 0
                    ? "Pearls: " + pearls + "/" + total
                    : "Pearls: -/-";

                // --- Medal ---
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

                // --- Play Button ---
                // Capture local variable for the closure
                string sceneToLoad = levelSceneNames[i];
                card.playButton.onClick.AddListener(() =>
                    SceneManager.LoadScene(sceneToLoad));
            }
            else
            {
                // Locked state display
                card.bestTimeText.text = "Complete previous level to unlock";
                card.pearlCountText.text = "";
                card.medalImage.gameObject.SetActive(false);
            }
        }
    }

    private void StyleButton(Button btn, bool unlocked)
    {
        btn.transition = Selectable.Transition.None;

        // Tint the button image to show locked/unlocked state
        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = unlocked ? Color.white : lockedButtonColor;

        // Keep button label text white in both states
        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null)
            tmp.color = buttonTextColor;
    }
}