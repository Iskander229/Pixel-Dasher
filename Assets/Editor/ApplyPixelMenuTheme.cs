#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class ApplyPixelMenuTheme
{
    private const string StartMenuPath = "Assets/Scenes/Start Menu.unity";
    private const string PauseMenuPath = "Assets/Resources/Pause Menu.prefab";
    private const string WorldMapPath = "Assets/Prefabs/World Map.prefab";
    private const string GroundEnemyPath = "Assets/Prefabs/GroundEnemy.prefab";
    private const string FloatEnemyPath = "Assets/Prefabs/FloatEnemy.prefab";
    private const string ThemeVersionKey = "PixelDasher2.MenuThemeVersion";
    private const int ThemeVersion = 4;

    private static readonly Color32 Black = new Color32(0, 0, 0, 255);
    private static readonly Color32 OffWhite = new Color32(245, 247, 240, 255);
    private static readonly Color32 Magenta = new Color32(211, 84, 197, 255);
    private static readonly Color32 Red = new Color32(255, 32, 32, 255);
    private static readonly Color32 Blue = new Color32(20, 30, 220, 255);
    private static readonly Color32 Gray = new Color32(115, 115, 115, 255);

    [InitializeOnLoadMethod]
    private static void ApplyOnceAfterImport()
    {
        if (EditorPrefs.GetInt(ThemeVersionKey, 0) < ThemeVersion)
        {
            EditorApplication.delayCall += () =>
            {
                ApplyAll();
                EditorPrefs.SetInt(ThemeVersionKey, ThemeVersion);
            };
        }
    }

    [MenuItem("Tools/Pixel Dasher/Apply Monochrome Menu Theme")]
    public static void ApplyAll()
    {
        ApplyStartMenuTheme();
        ApplyPauseMenuTheme();
        ApplyWorldMapTheme();
        AssetDatabase.SaveAssets();
        Debug.Log("Applied the Pixel Dasher monochrome menu theme.");
    }

    private static void ApplyStartMenuTheme()
    {
        if (!System.IO.File.Exists(StartMenuPath))
        {
            return;
        }

        Scene scene = SceneManager.GetSceneByPath(StartMenuPath);
        bool openedForTheme = !scene.IsValid() || !scene.isLoaded;
        if (openedForTheme)
        {
            scene = EditorSceneManager.OpenScene(StartMenuPath, OpenSceneMode.Additive);
        }

        Transform[] roots = scene.GetRootGameObjects().Select(item => item.transform).ToArray();
        SetImageColor(roots, "Background", Black);
        SetImageColor(roots, "Menu Panel", Black);
        AddOutline(Find(roots, "Menu Panel"), OffWhite, 5f);
        SetTextColor(roots, "Title", OffWhite);
        SetTextColor(roots, "Subtitle", Magenta);
        SetTextColor(roots, "Hint", OffWhite);
        StyleButton(roots, "Play Button", Magenta);
        StyleButton(roots, "Quit Button", Red);

        Transform canvas = Find(roots, "Menu Canvas");
        if (canvas != null)
        {
            AddFrameDecorations(canvas, "Start Theme Decorations", true);
            AddStartMenuEnemies(canvas);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        if (openedForTheme)
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static void ApplyPauseMenuTheme()
    {
        if (!System.IO.File.Exists(PauseMenuPath))
        {
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(PauseMenuPath);
        Transform[] roots = { root.transform };

        SetImageColor(roots, "Dimmer", new Color32(0, 0, 0, 225));
        SetImageColor(roots, "Pause Panel", Black);
        AddOutline(Find(roots, "Pause Panel"), OffWhite, 5f);
        SetTextColor(roots, "Title", OffWhite);
        SetTextColor(roots, "Hint", Magenta);
        StyleButton(roots, "Resume Button", Blue);
        StyleButton(roots, "Restart Button", Magenta);
        StyleButton(roots, "Main Menu Button", Magenta);
        SetButtonLabel(roots, "Main Menu Button", "WORLD MAP");
        StyleButton(roots, "Quit Button", Red);

        Transform dimmer = Find(roots, "Dimmer");
        if (dimmer != null)
        {
            AddFrameDecorations(dimmer, "Pause Theme Decorations", false);
        }

        PrefabUtility.SaveAsPrefabAsset(root, PauseMenuPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void ApplyWorldMapTheme()
    {
        if (!System.IO.File.Exists(WorldMapPath))
        {
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(WorldMapPath);
        Transform[] roots = { root.transform };

        SetImageColor(roots, "Background", Black);
        SetImageColor(roots, "Map Area", Black);
        AddOutline(Find(roots, "Map Area"), OffWhite, 5f);
        SetTextColor(roots, "Title", OffWhite);
        SetTextColor(roots, "Subtitle", Magenta);
        SetTextColor(roots, "Legend", OffWhite);
        StyleButton(roots, "Back Button", Magenta);

        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            if (image.name == "Level Path")
            {
                image.color = OffWhite;
            }
            else if (image.name == "Map Decoration")
            {
                image.color = Black;
                AddOutline(image.transform, OffWhite, 3f);
            }
            else if (image.name.EndsWith(" Node", StringComparison.Ordinal))
            {
                image.color = OffWhite;
                AddOutline(image.transform, OffWhite, 4f);
                StyleNodeButton(image.GetComponent<Button>());
            }
        }

        Transform mapArea = Find(roots, "Map Area");
        if (mapArea != null)
        {
            AddFrameDecorations(mapArea, "Map Theme Decorations", true);
            AddWorldMapEnemies(mapArea);
        }

        PrefabUtility.SaveAsPrefabAsset(root, WorldMapPath);
        PrefabUtility.UnloadPrefabContents(root);
    }

    private static void StyleButton(Transform[] roots, string objectName, Color accent)
    {
        Transform target = Find(roots, objectName);
        if (target == null)
        {
            return;
        }

        Image image = target.GetComponent<Image>();
        Button button = target.GetComponent<Button>();
        if (image != null)
        {
            image.color = Black;
        }

        AddOutline(target, OffWhite, 4f);
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Black;
            colors.highlightedColor = accent;
            colors.selectedColor = accent;
            colors.pressedColor = Gray;
            colors.disabledColor = new Color32(40, 40, 40, 255);
            colors.colorMultiplier = 1f;
            button.colors = colors;
        }

        Text label = target.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            label.color = OffWhite;
            AddOutline(label.transform, Black, 2f);
        }
    }

    private static void StyleNodeButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Magenta;
        colors.selectedColor = Blue;
        colors.pressedColor = Gray;
        colors.disabledColor = Color.white;
        colors.colorMultiplier = 1f;
        button.colors = colors;
    }

    private static void SetButtonLabel(Transform[] roots, string objectName, string label)
    {
        Transform target = Find(roots, objectName);
        Text text = target != null ? target.GetComponentInChildren<Text>(true) : null;
        if (text != null)
        {
            text.text = label;
        }
    }

    private static void AddFrameDecorations(Transform parent, string containerName, bool includeChain)
    {
        Transform existing = parent.Find(containerName);
        if (existing != null)
        {
            UnityEngine.Object.DestroyImmediate(existing.gameObject);
        }

        GameObject containerObject = new GameObject(containerName, typeof(RectTransform));
        containerObject.transform.SetParent(parent, false);
        RectTransform container = containerObject.GetComponent<RectTransform>();
        Stretch(container);
        container.SetAsLastSibling();

        CreateBar(container, "Top Border", new Vector2(0.5f, 1f), new Vector2(0f, -4f), new Vector2(0f, 8f), OffWhite, true);
        CreateBar(container, "Bottom Platform", new Vector2(0.5f, 0f), new Vector2(0f, 4f), new Vector2(0f, 8f), OffWhite, true);
        CreateBar(container, "Left Border", new Vector2(0f, 0.5f), new Vector2(4f, 0f), new Vector2(8f, 0f), OffWhite, false);
        CreateBar(container, "Right Border", new Vector2(1f, 0.5f), new Vector2(-4f, 0f), new Vector2(8f, 0f), OffWhite, false);

        CreateDiamond(container, new Vector2(-0.36f, 0.27f));
        CreateDiamond(container, new Vector2(0.37f, -0.28f));

        if (includeChain)
        {
            for (int index = 0; index < 7; index++)
            {
                Image link = CreateImage("Chain Link", container, OffWhite);
                RectTransform rect = link.rectTransform;
                rect.anchorMin = new Vector2(0.82f, 1f);
                rect.anchorMax = rect.anchorMin;
                rect.sizeDelta = new Vector2(12f, 20f);
                rect.anchoredPosition = new Vector2(0f, -18f - index * 25f);
                link.raycastTarget = false;

                Image hole = CreateImage("Chain Hole", rect, Black);
                RectTransform holeRect = hole.rectTransform;
                holeRect.anchorMin = new Vector2(0.5f, 0.5f);
                holeRect.anchorMax = holeRect.anchorMin;
                holeRect.sizeDelta = new Vector2(5f, 11f);
                holeRect.anchoredPosition = Vector2.zero;
                hole.raycastTarget = false;
            }
        }
    }

    private static void AddStartMenuEnemies(Transform canvas)
    {
        Transform container = RecreateContainer(canvas, "Start Menu Enemies");
        EnemyVisual groundEnemy = LoadEnemyVisual(GroundEnemyPath);
        EnemyVisual floatEnemy = LoadEnemyVisual(FloatEnemyPath);

        CreateEnemyDecoration(container, "Ground Enemy Decoration", groundEnemy, new Vector2(0.16f, 0f), new Vector2(0f, 74f), new Vector2(92f, 92f), true, 65f, 1.35f, 0f, groundEnemy.color);
        CreateEnemyDecoration(container, "Floating Enemy Decoration", floatEnemy, new Vector2(0.74f, 0.72f), Vector2.zero, new Vector2(88f, 88f), false, 16f, 2.1f, 0.6f, floatEnemy.color);
    }

    private static void AddWorldMapEnemies(Transform mapArea)
    {
        Transform container = RecreateContainer(mapArea, "World Map Enemies");
        EnemyVisual groundEnemy = LoadEnemyVisual(GroundEnemyPath);
        EnemyVisual floatEnemy = LoadEnemyVisual(FloatEnemyPath);

        CreateEnemyDecoration(container, "Level 1 Ground Enemy", groundEnemy, new Vector2(0.5f, 0.5f), new Vector2(-590f, -250f), new Vector2(74f, 74f), true, 24f, 1.5f, 0f, groundEnemy.color);
        CreateEnemyDecoration(container, "Level 2 Floating Enemy", floatEnemy, new Vector2(0.5f, 0.5f), new Vector2(-315f, 180f), new Vector2(72f, 72f), false, 13f, 2.2f, 0.8f, floatEnemy.color);

        Color32 silhouette = new Color32(105, 105, 105, 125);
        CreateEnemyDecoration(container, "Future Enemy 1", floatEnemy, new Vector2(0.5f, 0.5f), new Vector2(-30f, -190f), new Vector2(58f, 58f), false, 9f, 1.6f, 1.2f, silhouette);
        CreateEnemyDecoration(container, "Future Enemy 2", groundEnemy, new Vector2(0.5f, 0.5f), new Vector2(270f, 35f), new Vector2(58f, 58f), true, 13f, 1.2f, 0.4f, silhouette);
        CreateEnemyDecoration(container, "Future Enemy 3", floatEnemy, new Vector2(0.5f, 0.5f), new Vector2(575f, -155f), new Vector2(58f, 58f), false, 10f, 1.8f, 1.7f, silhouette);
    }

    private static Transform RecreateContainer(Transform parent, string name)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            UnityEngine.Object.DestroyImmediate(existing.gameObject);
        }

        GameObject containerObject = new GameObject(name, typeof(RectTransform));
        containerObject.transform.SetParent(parent, false);
        RectTransform rect = containerObject.GetComponent<RectTransform>();
        Stretch(rect);
        rect.SetAsLastSibling();
        return rect;
    }

    private static void CreateEnemyDecoration(Transform parent, string name, EnemyVisual visual, Vector2 anchor, Vector2 position, Vector2 size, bool horizontal, float distance, float speed, float phase, Color color)
    {
        if (visual.sprite == null)
        {
            return;
        }

        Image image = CreateImage(name, parent, color);
        image.sprite = visual.sprite;
        image.preserveAspect = true;
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        MenuEnemyDecoration animation = image.gameObject.AddComponent<MenuEnemyDecoration>();
        animation.Configure(horizontal, distance, speed, phase);
    }

    private static EnemyVisual LoadEnemyVisual(string prefabPath)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        SpriteRenderer renderer = prefab != null ? prefab.GetComponentInChildren<SpriteRenderer>(true) : null;
        return renderer != null
            ? new EnemyVisual(renderer.sprite, renderer.color)
            : new EnemyVisual(null, Color.white);
    }

    private readonly struct EnemyVisual
    {
        public readonly Sprite sprite;
        public readonly Color color;

        public EnemyVisual(Sprite sprite, Color color)
        {
            this.sprite = sprite;
            this.color = color;
        }
    }

    private static void CreateBar(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, Color color, bool horizontal)
    {
        Image bar = CreateImage(name, parent, color);
        RectTransform rect = bar.rectTransform;
        rect.anchorMin = horizontal ? new Vector2(0f, anchor.y) : new Vector2(anchor.x, 0f);
        rect.anchorMax = horizontal ? new Vector2(1f, anchor.y) : new Vector2(anchor.x, 1f);
        rect.pivot = anchor;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        bar.raycastTarget = false;
    }

    private static void CreateDiamond(Transform parent, Vector2 anchor)
    {
        Image diamond = CreateImage("Gem Accent", parent, Magenta);
        RectTransform rect = diamond.rectTransform;
        rect.anchorMin = new Vector2(0.5f + anchor.x, 0.5f + anchor.y);
        rect.anchorMax = rect.anchorMin;
        rect.sizeDelta = new Vector2(34f, 34f);
        rect.localEulerAngles = new Vector3(0f, 0f, 45f);
        diamond.raycastTarget = false;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static void AddOutline(Transform target, Color color, float size)
    {
        if (target == null || target.GetComponent<Graphic>() == null)
        {
            return;
        }

        Outline outline = target.GetComponent<Outline>();
        if (outline == null)
        {
            outline = target.gameObject.AddComponent<Outline>();
        }

        outline.effectColor = color;
        outline.effectDistance = new Vector2(size, -size);
        outline.useGraphicAlpha = true;
    }

    private static void SetImageColor(Transform[] roots, string name, Color color)
    {
        Transform target = Find(roots, name);
        Image image = target != null ? target.GetComponent<Image>() : null;
        if (image != null)
        {
            image.color = color;
        }
    }

    private static void SetTextColor(Transform[] roots, string name, Color color)
    {
        Transform target = Find(roots, name);
        Text text = target != null ? target.GetComponent<Text>() : null;
        if (text != null)
        {
            text.color = color;
        }
    }

    private static Transform Find(Transform[] roots, string name)
    {
        foreach (Transform root in roots)
        {
            Transform match = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(item => item.name == name);
            if (match != null)
            {
                return match;
            }
        }

        return null;
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
