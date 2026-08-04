#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CreateStartMenuScene
{
    private const string MenuScenePath = "Assets/Scenes/Start Menu.unity";

    [InitializeOnLoadMethod]
    private static void CreateOnceAfterImport()
    {
        if (!System.IO.File.Exists(MenuScenePath))
        {
            EditorApplication.delayCall += Create;
        }
    }

    [MenuItem("Tools/Pixel Dasher/Create Start Menu")]
    public static void Create()
    {
        Scene previousScene = SceneManager.GetActiveScene();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        scene.name = "Start Menu";

        Camera camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(9, 12, 22, 255);
        camera.orthographic = true;

        GameObject canvasObject = new GameObject("Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Image background = CreateImage("Background", canvasObject.transform, new Color32(9, 12, 22, 255));
        Stretch(background.rectTransform);

        Image panel = CreateImage("Menu Panel", canvasObject.transform, new Color32(20, 29, 48, 245));
        SetRect(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(620f, 620f), Vector2.zero);

        Text title = CreateText("Title", panel.transform, "PIXEL\nDASHER", 86, FontStyle.Bold, new Color32(103, 232, 255, 255));
        title.lineSpacing = 0.82f;
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(560f, 220f), new Vector2(0f, -145f));

        Text subtitle = CreateText("Subtitle", panel.transform, "COLLECT  •  DASH  •  ESCAPE", 25, FontStyle.Normal, new Color32(190, 202, 220, 255));
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(540f, 50f), new Vector2(0f, 65f));

        MainMenu menu = new GameObject("Main Menu Controller", typeof(MainMenu)).GetComponent<MainMenu>();

        Button play = CreateButton("Play Button", panel.transform, "PLAY", new Vector2(0f, -55f));
        UnityEventTools.AddPersistentListener(play.onClick, menu.PlayGame);

        Button quit = CreateButton("Quit Button", panel.transform, "QUIT", new Vector2(0f, -190f));
        UnityEventTools.AddPersistentListener(quit.onClick, menu.QuitGame);

        Text hint = CreateText("Hint", panel.transform, "Use mouse or keyboard to select", 20, FontStyle.Italic, new Color32(125, 142, 166, 255));
        SetRect(hint.rectTransform, new Vector2(0.5f, 0f), new Vector2(520f, 45f), new Vector2(0f, 42f));

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        eventSystem.GetComponent<EventSystem>().firstSelectedGameObject = play.gameObject;

        EditorSceneManager.SaveScene(scene, MenuScenePath);
        SceneManager.SetActiveScene(previousScene);
        EditorSceneManager.CloseScene(scene, true);

        EditorBuildSettingsScene[] existing = EditorBuildSettings.scenes
            .Where(item => item.path != MenuScenePath)
            .ToArray();
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MenuScenePath, true) }
            .Concat(existing)
            .ToArray();

        AssetDatabase.SaveAssets();
        Debug.Log("Pixel Dasher start menu created and added as the first build scene.");
    }

    private static Button CreateButton(string name, Transform parent, string label, Vector2 position)
    {
        Image image = CreateImage(name, parent, new Color32(43, 72, 105, 255));
        SetRect(image.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(390f, 92f), position);

        Button button = image.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(103, 232, 255, 255);
        colors.pressedColor = new Color32(65, 170, 200, 255);
        colors.selectedColor = new Color32(103, 232, 255, 255);
        button.colors = colors;

        Text text = CreateText("Label", image.transform, label, 38, FontStyle.Bold, Color.white);
        Stretch(text.rectTransform);
        return button;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(string name, Transform parent, string value, int size, FontStyle style, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        gameObject.transform.SetParent(parent, false);
        Text text = gameObject.GetComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 size, Vector2 position)
    {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
#endif
