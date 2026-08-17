#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CreateWorldMap
{
    private const string PrefabPath = "Assets/Prefabs/World Map.prefab";
    private const string ScenePath = "Assets/Scenes/World Map.unity";

    [InitializeOnLoadMethod]
    private static void CreateOnceAfterImport()
    {
        if (!File.Exists(PrefabPath) || !File.Exists(ScenePath))
        {
            EditorApplication.delayCall += Create;
        }
    }

    [MenuItem("Tools/Pixel Dasher/Create World Map")]
    public static void Create()
    {
        GameObject mapPrefab = BuildMapInterface();
        PrefabUtility.SaveAsPrefabAsset(mapPrefab, PrefabPath);
        Object.DestroyImmediate(mapPrefab);

        Scene previousScene = SceneManager.GetActiveScene();
        Scene mapScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(mapScene);
        mapScene.name = "World Map";

        Camera camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(7, 11, 22, 255);
        camera.orthographic = true;

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        PrefabUtility.InstantiatePrefab(prefab, mapScene);

        GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        SceneManager.MoveGameObjectToScene(eventSystem, mapScene);

        EditorSceneManager.SaveScene(mapScene, ScenePath);
        SceneManager.SetActiveScene(previousScene);
        EditorSceneManager.CloseScene(mapScene, true);

        AddSceneToBuildSettings();
        AssetDatabase.SaveAssets();
        ApplyPixelMenuTheme.ApplyAll();
        Debug.Log("Editable World Map prefab and scene created.");
    }

    private static GameObject BuildMapInterface()
    {
        GameObject root = new GameObject("World Map UI", typeof(WorldMapController));
        WorldMapController controller = root.GetComponent<WorldMapController>();

        GameObject canvasObject = new GameObject("World Map Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(root.transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Image background = CreateImage("Background", canvasObject.transform, new Color32(7, 11, 22, 255));
        Stretch(background.rectTransform);

        CreateDecoration(background.transform, new Vector2(-780f, 350f), new Vector2(260f, 150f), new Color32(20, 49, 70, 255));
        CreateDecoration(background.transform, new Vector2(730f, 300f), new Vector2(340f, 180f), new Color32(32, 42, 77, 255));
        CreateDecoration(background.transform, new Vector2(-680f, -360f), new Vector2(390f, 190f), new Color32(24, 62, 64, 255));
        CreateDecoration(background.transform, new Vector2(700f, -360f), new Vector2(300f, 150f), new Color32(46, 38, 68, 255));

        Text title = CreateText("Title", background.transform, "PIXEL DASHER WORLD", 62, FontStyle.Bold, new Color32(103, 232, 255, 255));
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(1100f, 100f), new Vector2(0f, -70f));

        Text subtitle = CreateText("Subtitle", background.transform, "CHOOSE YOUR NEXT RUN", 24, FontStyle.Normal, new Color32(172, 190, 214, 255));
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(800f, 50f), new Vector2(0f, -145f));

        Image mapPanel = CreateImage("Map Area", background.transform, new Color32(14, 23, 39, 235));
        SetRect(mapPanel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1740f, 720f), new Vector2(0f, -25f));

        Vector2[] positions =
        {
            new Vector2(-710f, -180f),
            new Vector2(-430f, 80f),
            new Vector2(-140f, -80f),
            new Vector2(155f, 130f),
            new Vector2(455f, -50f),
            new Vector2(710f, 185f)
        };

        for (int index = 0; index < positions.Length - 1; index++)
        {
            CreatePath(mapPanel.transform, positions[index], positions[index + 1]);
        }

        WorldMapLevelNode[] nodes = new WorldMapLevelNode[6];
        nodes[0] = CreateNode(mapPanel.transform, controller, 0, "level 0", false, positions[0]);
        nodes[1] = CreateNode(mapPanel.transform, controller, 1, "level 1", false, positions[1]);
        nodes[2] = CreateNode(mapPanel.transform, controller, 2, string.Empty, true, positions[2]);
        nodes[3] = CreateNode(mapPanel.transform, controller, 3, string.Empty, true, positions[3]);
        nodes[4] = CreateNode(mapPanel.transform, controller, 4, string.Empty, true, positions[4]);
        nodes[5] = CreateNode(mapPanel.transform, controller, 5, string.Empty, true, positions[5]);

        Button backButton = CreateButton("Back Button", background.transform, "BACK", new Vector2(-790f, -470f), new Vector2(250f, 72f));

        Text legend = CreateText("Legend", background.transform, "CYAN: AVAILABLE     GREEN: COMPLETE     GRAY: LOCKED / COMING SOON", 19, FontStyle.Normal, new Color32(125, 142, 166, 255));
        SetRect(legend.rectTransform, new Vector2(0.5f, 0f), new Vector2(1050f, 45f), new Vector2(180f, 32f));

        SerializedObject serializedController = new SerializedObject(controller);
        SerializedProperty nodesProperty = serializedController.FindProperty("levelNodes");
        nodesProperty.arraySize = nodes.Length;
        for (int index = 0; index < nodes.Length; index++)
        {
            nodesProperty.GetArrayElementAtIndex(index).objectReferenceValue = nodes[index];
        }

        serializedController.FindProperty("backButton").objectReferenceValue = backButton;
        serializedController.ApplyModifiedPropertiesWithoutUndo();
        return root;
    }

    private static WorldMapLevelNode CreateNode(Transform parent, WorldMapController controller, int levelIndex, string sceneName, bool comingSoon, Vector2 position)
    {
        Image nodeImage = CreateImage("Level " + (levelIndex + 1) + " Node", parent, new Color32(43, 126, 174, 255));
        SetRect(nodeImage.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(190f, 150f), position);

        Button button = nodeImage.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(103, 232, 255, 255);
        colors.pressedColor = new Color32(54, 157, 190, 255);
        colors.selectedColor = new Color32(103, 232, 255, 255);
        colors.disabledColor = new Color32(125, 125, 125, 210);
        button.colors = colors;

        Text title = CreateText("Level Name", nodeImage.transform, "LEVEL " + (levelIndex + 1), 27, FontStyle.Bold, Color.white);
        SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(180f, 50f), new Vector2(0f, 20f));

        Text status = CreateText("Status", nodeImage.transform, comingSoon ? "COMING SOON" : "AVAILABLE", 16, FontStyle.Normal, new Color32(221, 231, 243, 255));
        SetRect(status.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(180f, 40f), new Vector2(0f, -35f));

        WorldMapLevelNode node = nodeImage.gameObject.AddComponent<WorldMapLevelNode>();
        SerializedObject serializedNode = new SerializedObject(node);
        serializedNode.FindProperty("levelIndex").intValue = levelIndex;
        serializedNode.FindProperty("sceneName").stringValue = sceneName;
        serializedNode.FindProperty("comingSoon").boolValue = comingSoon;
        serializedNode.FindProperty("button").objectReferenceValue = button;
        serializedNode.FindProperty("nodeImage").objectReferenceValue = nodeImage;
        serializedNode.FindProperty("titleText").objectReferenceValue = title;
        serializedNode.FindProperty("statusText").objectReferenceValue = status;
        serializedNode.ApplyModifiedPropertiesWithoutUndo();
        return node;
    }

    private static void CreatePath(Transform parent, Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        Image path = CreateImage("Level Path", parent, new Color32(67, 83, 105, 255));
        SetRect(path.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(direction.magnitude, 16f), (from + to) * 0.5f);
        path.rectTransform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    private static void CreateDecoration(Transform parent, Vector2 position, Vector2 size, Color color)
    {
        Image decoration = CreateImage("Map Decoration", parent, color);
        SetRect(decoration.rectTransform, new Vector2(0.5f, 0.5f), size, position);
        decoration.raycastTarget = false;
    }

    private static Button CreateButton(string name, Transform parent, string label, Vector2 position, Vector2 size)
    {
        Image image = CreateImage(name, parent, new Color32(43, 72, 105, 255));
        SetRect(image.rectTransform, new Vector2(0.5f, 0.5f), size, position);
        Button button = image.gameObject.AddComponent<Button>();

        Text text = CreateText("Label", image.transform, label, 27, FontStyle.Bold, Color.white);
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

    private static void AddSceneToBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes
            .Where(scene => scene.path != ScenePath)
            .ToList();

        int menuIndex = scenes.FindIndex(scene => scene.path == "Assets/Scenes/Start Menu.unity");
        scenes.Insert(menuIndex >= 0 ? menuIndex + 1 : 0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
#endif
