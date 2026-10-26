using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public static class PrepareTeamIntegration
{
    [MenuItem("Tools/Transport Network Layers/Prepare Team Integration")]
    public static void Prepare()
    {
        if (Application.unityVersion != "2022.3.62f3")
            throw new InvalidOperationException("Use Unity 2022.3.62f3 for this export.");
        PlayerSettings.defaultScreenWidth = 1200;
        PlayerSettings.defaultScreenHeight = 700;
        PlayerSettings.defaultWebScreenWidth = 1200;
        PlayerSettings.defaultWebScreenHeight = 700;
        PlayerSettings.defaultIsNativeResolution = false;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = false;
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        if (lit == null) throw new InvalidOperationException("Install URP 14.0.12 before exporting.");
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("TextMesh Pro") || path.Contains("Skyboxes")) continue;
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            Color color = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor") : material.color;
            Texture texture = material.HasProperty("_BaseMap") ? material.GetTexture("_BaseMap") : material.mainTexture;
            Vector2 scale = material.HasProperty("_BaseMap") ? material.GetTextureScale("_BaseMap") : material.mainTextureScale;
            material.shader = lit;
            material.SetColor("_BaseColor", color);
            material.SetColor("_Color", color);
            material.SetTexture("_BaseMap", texture);
            material.SetTextureScale("_BaseMap", scale);
            EditorUtility.SetDirty(material);
        }
        foreach (string name in new[] { "TransportLayer", "NetworkLayer" })
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/" + name + ".unity");
            var manager = UnityEngine.Object.FindObjectOfType<GameManager>();
            var network = UnityEngine.Object.FindObjectOfType<NetworkGameManager>();
            GameObject panel = manager != null ? manager.factPanel : network.factPanel;
            Text fact = manager != null ? manager.factText : network.factText;
            foreach (Button button in panel.GetComponentsInChildren<Button>(true))
                UnityEngine.Object.DestroyImmediate(button.gameObject);
            for (int i = panel.transform.childCount - 1; i >= 0; i--)
                if (panel.transform.GetChild(i).name == "LevelNavigation")
                    UnityEngine.Object.DestroyImmediate(panel.transform.GetChild(i).gameObject);
            EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
                eventSystem = new GameObject("EventSystem", typeof(EventSystem)).GetComponent<EventSystem>();
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(eventSystem.gameObject);
            foreach (BaseInputModule module in eventSystem.GetComponents<BaseInputModule>())
                UnityEngine.Object.DestroyImmediate(module);
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
            Canvas canvas = panel.GetComponentInParent<Canvas>();
            canvas.sortingOrder = 20;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200, 700);
            scaler.matchWidthOrHeight = 0.5f;
            if (manager != null)
            {
                FirstPersonHud.StyleSceneCounter(manager.scoreText, false);
                FirstPersonHud.StyleSceneCounter(manager.timerText, true);
                FirstPersonHud.StyleSceneCenterCounter(manager.protocolText);
            }
            else
            {
                FirstPersonHud.StyleSceneCounter(network.statusText, false);
                FirstPersonHud.StyleSceneCounter(network.timerText, true);
                Text hint = GameObject.Find("ProtocolText").GetComponent<Text>();
                FirstPersonHud.StyleSceneCenterCounter(hint);
            }
            FirstPersonHud.StyleResultPanel(panel, fact);
            RectTransform row = new GameObject("LevelNavigation", typeof(RectTransform)).GetComponent<RectTransform>();
            row.SetParent(panel.transform, false);
            row.anchorMin = row.anchorMax = new Vector2(0.5f, 0.5f);
            row.sizeDelta = new Vector2(760, 56);
            row.anchoredPosition = new Vector2(0, -150);
            AddButton(row, "RestartLevelButton", "RESTART LEVEL", -260, new Color32(30, 103, 146, 255));
            AddButton(row, "NextLevelButton", "NEXT LEVEL", 0, new Color32(28, 126, 108, 255));
            AddButton(row, "LevelMenuButton", "LEVEL MENU", 260, new Color32(81, 74, 131, 255));
            FirstPersonHud.Create(name);
            panel.SetActive(false);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        try { SetGameViewSize(); }
        catch (Exception ex) { Debug.LogWarning("Game view preset could not be added: " + ex.Message); }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string output = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
        Directory.CreateDirectory(output);
        foreach (string name in new[] { "TransportLayer", "NetworkLayer" })
        {
            string[] common = {
                "Assets/Scripts", "Assets/Editor/ApplyLayerGamePlayerSettings.cs",
                "Assets/TextMesh Pro", "Assets/Skyboxes", "Assets/Materials"
            };
            string[] seeds = common.Concat(new[] {
                "Assets/Scenes/" + name + ".unity",
                "Assets/Resources/LayerMusic/" + name,
                "Assets/Resources/LayerMusic/CREDITS.txt"
            }).ToArray();
            // Resources music is seeded explicitly; native dependencies collect scene materials/textures.
            AssetDatabase.ExportPackage(seeds, Path.Combine(output, name + ".unitypackage"),
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        }
        Debug.Log("TEAM_INTEGRATION_EXPORTED: " + Application.unityVersion);
    }

    private static Button AddButton(Transform parent, string name, string title, float x, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(240, 56);
        rect.anchoredPosition = new Vector2(x, 0);
        Image image = go.GetComponent<Image>();
        image.color = color;
        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1.18f, 1.18f, 1.18f);
        colors.pressedColor = new Color(.72f, .72f, .72f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        button.onClick = new Button.ButtonClickedEvent();
        GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(go.transform, false);
        RectTransform textRect = label.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8, 4);
        textRect.offsetMax = new Vector2(-8, -4);
        Text text = label.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 21;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = title;
        text.raycastTarget = false;
        return button; // All three buttons intentionally have empty On Click lists.
    }

    private static void SetGameViewSize()
    {
        // Editor-only convenience; this is not a global change to the teammate's Game view.
        Type sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance").GetValue(null, null);
        object group = sizesType.GetMethod("GetGroup").Invoke(sizes, new object[] { 0 });
        Type sizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        Type sizeEnum = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, new[] { Enum.ToObject(sizeEnum, 1), (object)1200, 700, "Team 1200 x 700" });
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
    }
}
