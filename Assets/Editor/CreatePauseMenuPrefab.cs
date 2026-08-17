#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public static class CreatePauseMenuPrefab
{
    private const string ResourcesFolder = "Assets/Resources";
    private const string PrefabPath = ResourcesFolder + "/Pause Menu.prefab";

    [InitializeOnLoadMethod]
    private static void CreateOnceAfterImport()
    {
        if (!File.Exists(PrefabPath))
        {
            EditorApplication.delayCall += Create;
        }
    }

    [MenuItem("Tools/Pixel Dasher/Create Pause Menu Prefab")]
    public static void Create()
    {
        if (!AssetDatabase.IsValidFolder(ResourcesFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        GameObject root = new GameObject("Pause Menu", typeof(PauseMenu));
        PauseMenu controller = root.GetComponent<PauseMenu>();

        GameObject canvasObject = new GameObject("Pause Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(root.transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Image dimmer = CreateImage("Dimmer", canvasObject.transform, new Color32(5, 8, 15, 205));
        Stretch(dimmer.rectTransform);

        Image panel = CreateImage("Pause Panel", dimmer.transform, new Color32(20, 29, 48, 255));
        SetRect(panel.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(610f, 720f), Vector2.zero);

        Text title = CreateText("Title", panel.transform, "PAUSED", 78, FontStyle.Bold, new Color32(103, 232, 255, 255));
        SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(530f, 120f), new Vector2(0f, -100f));

        Button resume = CreateButton("Resume Button", panel.transform, "RESUME", new Vector2(0f, 125f));
        Button restart = CreateButton("Restart Button", panel.transform, "RESTART LEVEL", new Vector2(0f, 5f));
        Button mainMenu = CreateButton("Main Menu Button", panel.transform, "WORLD MAP", new Vector2(0f, -115f));
        Button quit = CreateButton("Quit Button", panel.transform, "QUIT GAME", new Vector2(0f, -235f));

        UnityEventTools.AddPersistentListener(resume.onClick, controller.Resume);
        UnityEventTools.AddPersistentListener(restart.onClick, controller.RestartLevel);
        UnityEventTools.AddPersistentListener(mainMenu.onClick, controller.ReturnToWorldMap);
        UnityEventTools.AddPersistentListener(quit.onClick, controller.QuitGame);

        Text hint = CreateText("Hint", panel.transform, "ESC / START TO RESUME", 21, FontStyle.Normal, new Color32(125, 142, 166, 255));
        SetRect(hint.rectTransform, new Vector2(0.5f, 0f), new Vector2(520f, 45f), new Vector2(0f, 38f));

        SerializedObject serializedController = new SerializedObject(controller);
        serializedController.FindProperty("pausePanel").objectReferenceValue = dimmer.gameObject;
        serializedController.FindProperty("resumeButton").objectReferenceValue = resume;
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        dimmer.gameObject.SetActive(false);
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        ApplyPixelMenuTheme.ApplyAll();
        Debug.Log("Editable Pause Menu prefab created at " + PrefabPath);
    }

    private static Button CreateButton(string name, Transform parent, string label, Vector2 position)
    {
        Image image = CreateImage(name, parent, new Color32(43, 72, 105, 255));
        SetRect(image.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(410f, 86f), position);

        Button button = image.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color32(103, 232, 255, 255);
        colors.pressedColor = new Color32(65, 170, 200, 255);
        colors.selectedColor = new Color32(103, 232, 255, 255);
        button.colors = colors;

        Text text = CreateText("Label", image.transform, label, 32, FontStyle.Bold, Color.white);
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
}
#endif
