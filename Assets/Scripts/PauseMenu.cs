using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private const string MenuSceneName = "Start Menu";
    private const string PrefabResourceName = "Pause Menu";

    private static PauseMenu instance;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;

    private bool isPaused;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        if (scene.name == MenuSceneName)
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }

            return;
        }

        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>(PrefabResourceName);
            if (prefab == null)
            {
                Debug.LogError("Pause Menu prefab was not found in an Assets/Resources folder.");
                return;
            }

            GameObject pauseMenuObject = Instantiate(prefab);
            pauseMenuObject.name = "Pause Menu";
            instance = pauseMenuObject.GetComponent<PauseMenu>();
            DontDestroyOnLoad(pauseMenuObject);
        }

        instance.SetPaused(false);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        bool keyboardPause = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool gamepadPause = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (keyboardPause || gamepadPause)
        {
            SetPaused(!isPaused);
        }
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void RestartLevel()
    {
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene(MenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        pausePanel.SetActive(paused);

        if (paused && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            Time.timeScale = 1f;
        }
    }
}
