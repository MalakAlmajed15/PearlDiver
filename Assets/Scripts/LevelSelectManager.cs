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

    [Header("Button Style")]
    public Color lockedButtonColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color buttonTextColor = Color.white;

    
    //public Color playButtonColor = new Color(0.514f, 0.608f, 0.816f, 1f);

    private string[] levelNames = {
        "Shallow Reef",
        "Coral Garden",
        "Deep Cave",
        "Treasure Cove"
    };

    void Start()
    {
        backButton.onClick.AddListener(() =>
            SceneManager.LoadScene(mainMenuSceneName));

        for (int i = 0; i < 5; i++)
        {
            int levelIndex = i + 1;
            LevelCard card = levelCards[i];
            bool unlocked = GameData.IsUnlocked(levelIndex);

            card.playButton.interactable = unlocked;

          
            StyleButton(card.playButton, unlocked);

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

    private void StyleButton(Button btn, bool unlocked)
    {
   
        btn.transition = Selectable.Transition.None;

        //Image img = btn.GetComponent<Image>();
        //if (img != null)
        //{
        //    img.color = unlocked ? playButtonColor : lockedButtonColor;
        //}

       
        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmp != null)
            tmp.color = buttonTextColor;
    }
}