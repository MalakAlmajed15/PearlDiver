using UnityEngine;

public static class GameData
{
    // Call this when a level is completed
    public static void SaveLevelResult(int levelIndex, float time, int pearls, int totalPearls)
    {
        string prefix = "Level" + levelIndex;

        // Unlock this level and the next
        PlayerPrefs.SetInt(prefix + "_Unlocked", 1);
        if (levelIndex < 5)
            PlayerPrefs.SetInt("Level" + (levelIndex + 1) + "_Unlocked", 1);

        // Save best time (lower is better)
        float savedTime = PlayerPrefs.GetFloat(prefix + "_BestTime", float.MaxValue);
        if (time < savedTime)
            PlayerPrefs.SetFloat(prefix + "_BestTime", time);

        // Save best pearl count (higher is better)
        int savedPearls = PlayerPrefs.GetInt(prefix + "_Pearls", 0);
        if (pearls > savedPearls)
            PlayerPrefs.SetInt(prefix + "_Pearls", pearls);

        PlayerPrefs.SetInt(prefix + "_TotalPearls", totalPearls);
        PlayerPrefs.Save();
    }

    public static bool IsUnlocked(int levelIndex)
    {
        if (levelIndex == 1) return true; // Level 1 ALWAYS unlocked
        return PlayerPrefs.GetInt("Level" + levelIndex + "_Unlocked", 0) == 1;
    }

    public static float GetBestTime(int levelIndex)
    {
        return PlayerPrefs.GetFloat("Level" + levelIndex + "_BestTime", -1f);
    }

    public static int GetPearls(int levelIndex)
    {
        return PlayerPrefs.GetInt("Level" + levelIndex + "_Pearls", 0);
    }

    public static int GetTotalPearls(int levelIndex)
    {
        return PlayerPrefs.GetInt("Level" + levelIndex + "_TotalPearls", 0);
    }

    // Medal based on time thresholds (per your GDD level durations)
    // Gold / Silver / Bronze time limits in seconds per level
    public static int GetMedal(int levelIndex)
    {
        float best = GetBestTime(levelIndex);
        if (best < 0) return 0; // No medal yet

        float[] gold = { 120f, 150f, 180f, 210f, 240f };
        float[] silver = { 180f, 220f, 270f, 310f, 360f };
        float[] bronze = { 240f, 300f, 360f, 420f, 480f };

        int i = levelIndex - 1;
        if (best <= gold[i]) return 3; // Gold
        if (best <= silver[i]) return 2; // Silver
        if (best <= bronze[i]) return 1; // Bronze
        return 0;
    }

    public static string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60);
        int s = Mathf.FloorToInt(seconds % 60);
        return string.Format("{0}:{1:00}", m, s);
    }
}