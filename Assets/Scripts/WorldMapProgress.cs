using UnityEngine;
using UnityEngine.SceneManagement;

public static class WorldMapProgress
{
    private const string HighestUnlockedKey = "WorldMap.HighestUnlocked";
    private const string SelectedLevelKey = "WorldMap.SelectedLevel";
    private const string CompletedKeyPrefix = "WorldMap.Completed.";
    private const string BestGemsKeyPrefix = "WorldMap.BestGems.";
    private const int FirstGameplayBuildIndex = 2;

    public static int HighestUnlockedLevel => PlayerPrefs.GetInt(HighestUnlockedKey, 0);

    public static void SelectLevel(int levelIndex)
    {
        PlayerPrefs.SetInt(SelectedLevelKey, levelIndex);
        PlayerPrefs.Save();
    }

    public static bool IsCompleted(int levelIndex)
    {
        return PlayerPrefs.GetInt(CompletedKeyPrefix + levelIndex, 0) == 1;
    }

    public static int GetBestGemProgress(int levelIndex)
    {
        return PlayerPrefs.GetInt(BestGemsKeyPrefix + levelIndex, 0);
    }

    public static void CompleteCurrentLevel(int gemProgress)
    {
        int fallbackLevel = Mathf.Max(0, SceneManager.GetActiveScene().buildIndex - FirstGameplayBuildIndex);
        int levelIndex = PlayerPrefs.GetInt(SelectedLevelKey, fallbackLevel);

        PlayerPrefs.SetInt(CompletedKeyPrefix + levelIndex, 1);
        PlayerPrefs.SetInt(BestGemsKeyPrefix + levelIndex, Mathf.Max(gemProgress, GetBestGemProgress(levelIndex)));
        PlayerPrefs.SetInt(HighestUnlockedKey, Mathf.Max(HighestUnlockedLevel, levelIndex + 1));
        PlayerPrefs.Save();
    }
}
