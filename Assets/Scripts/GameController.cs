using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private const int FirstGameplaySceneIndex = 1;

    private int progressAmount;

    public Slider progressSlider;
    public GameObject player;
    public GameObject LoadCanvas;

    private void OnEnable()
    {
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadNextLevel.OnHoldComplete += LoadNextLevel;
    }

    private void OnDisable()
    {
        Gem.OnGemCollect -= IncreaseProgressAmount;
        HoldToLoadNextLevel.OnHoldComplete -= LoadNextLevel;
    }

    private void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        LoadCanvas.SetActive(false);
    }

    private void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        Debug.Log($"Gem collected! progress: {progressAmount}");
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            LoadCanvas.SetActive(true);
            Debug.Log("level complete!");
        }
    }

    private void LoadNextLevel()
    {
        LoadCanvas.SetActive(false);

        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = FirstGameplaySceneIndex;
        }

        SceneManager.LoadScene(nextLevelIndex);
    }
}
