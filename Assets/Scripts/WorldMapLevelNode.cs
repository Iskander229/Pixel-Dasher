using UnityEngine;
using UnityEngine.UI;

public class WorldMapLevelNode : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private string sceneName;
    [SerializeField] private bool comingSoon;
    [SerializeField] private Button button;
    [SerializeField] private Image nodeImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Text statusText;

    private WorldMapController controller;

    public int LevelIndex => levelIndex;
    public string SceneName => sceneName;
    public bool ComingSoon => comingSoon;
    public Button Button => button;

    public void Initialize(WorldMapController owningController)
    {
        controller = owningController;
        button.onClick.RemoveListener(SelectLevel);
        button.onClick.AddListener(SelectLevel);

        bool completed = WorldMapProgress.IsCompleted(levelIndex);
        bool unlocked = levelIndex <= WorldMapProgress.HighestUnlockedLevel;
        int bestGems = WorldMapProgress.GetBestGemProgress(levelIndex);

        titleText.text = "LEVEL " + (levelIndex + 1);

        if (comingSoon)
        {
            button.interactable = false;
            nodeImage.color = new Color32(169, 26, 207, 255);
            titleText.color = Color.white;
            statusText.color = Color.white;
            statusText.text = "COMING SOON";
        }
        else if (!unlocked)
        {
            button.interactable = false;
            nodeImage.color = new Color32(92, 92, 92, 255);
            titleText.color = Color.white;
            statusText.color = Color.white;
            statusText.text = "LOCKED";
        }
        else if (completed)
        {
            button.interactable = true;
            nodeImage.color = new Color32(0, 227, 76, 255);
            titleText.color = Color.black;
            statusText.color = Color.black;
            statusText.text = bestGems + "%  COMPLETE";
        }
        else
        {
            button.interactable = true;
            nodeImage.color = new Color32(245, 247, 240, 255);
            titleText.color = Color.black;
            statusText.color = Color.black;
            statusText.text = "AVAILABLE";
        }
    }

    private void SelectLevel()
    {
        controller.OpenLevel(this);
    }
}
