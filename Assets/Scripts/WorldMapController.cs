using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMapController : MonoBehaviour
{
    [SerializeField] private WorldMapLevelNode[] levelNodes;
    [SerializeField] private Button backButton;

    private void Start()
    {
        WorldMapLevelNode firstAvailable = null;

        foreach (WorldMapLevelNode node in levelNodes)
        {
            node.Initialize(this);

            if (firstAvailable == null && node.Button.interactable)
            {
                firstAvailable = node;
            }
        }

        backButton.onClick.RemoveListener(ReturnToMainMenu);
        backButton.onClick.AddListener(ReturnToMainMenu);

        if (firstAvailable != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(firstAvailable.Button.gameObject);
        }
    }

    private void Update()
    {
        bool keyboardBack = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool gamepadBack = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;

        if (keyboardBack || gamepadBack)
        {
            ReturnToMainMenu();
        }
    }

    public void OpenLevel(WorldMapLevelNode node)
    {
        if (node.ComingSoon || string.IsNullOrWhiteSpace(node.SceneName))
        {
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(node.SceneName))
        {
            Debug.LogWarning("World map scene is not available in Build Settings: " + node.SceneName);
            return;
        }

        WorldMapProgress.SelectLevel(node.LevelIndex);
        SceneManager.LoadScene(node.SceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }
}
