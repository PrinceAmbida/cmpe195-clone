using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WorldObjectLabeler
{
    private struct LabelInfo
    {
        public string title;
        public string description;
        public Color accent;

        public LabelInfo(string title, string description, Color accent)
        {
            this.title = title;
            this.description = description;
            this.accent = accent;
        }
    }

    private static readonly Color Cyan = new Color32(91, 224, 255, 255);
    private static readonly Color Green = new Color32(94, 237, 175, 255);
    private static readonly Color Red = new Color32(255, 102, 126, 255);
    private static readonly Color Blue = new Color32(132, 174, 255, 255);
    private static readonly Color Gold = new Color32(255, 211, 112, 255);

    public static void CreateLabels(Camera mainCamera)
    {
        if (mainCamera == null || GameObject.Find("WorldInfoCanvas") != null)
        {
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        bool isNetworkScene = sceneName == "NetworkLayer";
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        GameObject canvasObject = new GameObject(
            "WorldInfoCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(UnityEngine.UI.CanvasScaler)
        );
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = -1;
        UnityEngine.UI.CanvasScaler scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1f;
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();

        Transform[] sceneObjects = Object.FindObjectsOfType<Transform>();
        foreach (Transform target in sceneObjects)
        {
            if (target == null ||
                target.GetComponentInChildren<Renderer>() == null ||
                !TryGetLabel(target.name.Trim(), isNetworkScene, out LabelInfo info))
            {
                continue;
            }

            ApplyTint(target, info.accent);

            GameObject labelObject = new GameObject(
                "WorldInfoLabel_" + target.name,
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(UnityEngine.UI.Image)
            );
            labelObject.transform.SetParent(canvasObject.transform, false);
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0f);
            labelRect.sizeDelta = new Vector2(380f, 126f);

            UnityEngine.UI.Image background = labelObject.GetComponent<UnityEngine.UI.Image>();
            background.color = new Color(0.012f, 0.035f, 0.075f, 0.88f);
            background.raycastTarget = false;
            CanvasGroup group = labelObject.GetComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.interactable = false;

            GameObject textObject = new GameObject("LabelText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(labelObject.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12f, 8f);
            textRect.offsetMax = new Vector2(-12f, -8f);

            TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
            label.text = "<b><color=#" + ColorUtility.ToHtmlStringRGB(info.accent) + ">" +
                         info.title + "</color></b>\n<size=80%>" + info.description + "</size>";
            if (font != null)
            {
                label.font = font;
            }
            label.fontSize = 30f;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
            label.enableWordWrapping = true;
            label.raycastTarget = false;

            WorldScreenLabel tracker = labelObject.AddComponent<WorldScreenLabel>();
            tracker.Initialize(target, mainCamera, canvasRect, GetLabelOffset(target));
        }
    }

    private static bool TryGetLabel(string objectName, bool isNetworkScene, out LabelInfo info)
    {
        if (!isNetworkScene)
        {
            if (objectName.StartsWith("TCP_Segment"))
            {
                info = new LabelInfo("TCP SEGMENT", "Reliable, ordered delivery.", Cyan);
                return true;
            }
            if (objectName.StartsWith("UDP_Segment"))
            {
                info = new LabelInfo("UDP DATAGRAM", "Fast delivery without guarantee.", Green);
                return true;
            }
            if (objectName.StartsWith("DoS_Enemy"))
            {
                info = new LabelInfo("DoS ATTACK", "Floods a service with fake requests.", Red);
                return true;
            }
            info = default;
            return false;
        }

        if (objectName == "StartPoint")
        {
            info = new LabelInfo("START CHECKPOINT", "The packet begins its route here.", Cyan);
            return true;
        }
        if (objectName == "SafePath2")
        {
            info = new LabelInfo("RISKY ROUTE", "MITM attackers patrol this path.", Red);
            return true;
        }
        if (objectName.StartsWith("SafePath"))
        {
            info = new LabelInfo("SAFE ROUTE", "Lower-risk route to the next hop.", Green);
            return true;
        }
        if (objectName.StartsWith("DangerPath"))
        {
            info = new LabelInfo("RISKY ROUTE", "MITM attackers patrol this path.", Red);
            return true;
        }
        if (objectName.StartsWith("MITM_Enemy"))
        {
            info = new LabelInfo("MITM ATTACKER", "Intercepts packets; sends you to a checkpoint.", Red);
            return true;
        }
        if (objectName == "GoalZone")
        {
            info = new LabelInfo("DESTINATION", "Reach this zone after Routers 1, 2 and 3.", Gold);
            return true;
        }
        for (int router = 1; router <= 3; router++)
        {
            if (objectName == "Router" + router)
            {
                info = new LabelInfo("ROUTER " + router, "Forwards IP packets. Saves a checkpoint.", Blue);
                return true;
            }
            if (objectName == "Router" + router + "Point")
            {
                info = new LabelInfo("CHECKPOINT " + router, "Reaching this router sets your respawn point.", Gold);
                return true;
            }
        }

        info = default;
        return false;
    }

    private static Vector3 GetLabelOffset(Transform target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        bool hasBounds = false;
        Bounds bounds = new Bounds(target.position, Vector3.zero);
        foreach (Renderer currentRenderer in renderers)
        {
            if (currentRenderer == null)
            {
                continue;
            }
            if (!hasBounds)
            {
                bounds = currentRenderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(currentRenderer.bounds);
            }
        }

        float height = hasBounds ? Mathf.Max(0.9f, bounds.max.y - target.position.y + 0.45f) : 1.5f;
        return Vector3.up * height;
    }

    private static void ApplyTint(Transform target, Color tint)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        foreach (Renderer currentRenderer in renderers)
        {
            if (currentRenderer == null)
            {
                continue;
            }

            bool hasColorProperty = false;
            foreach (Material material in currentRenderer.sharedMaterials)
            {
                if (material == null)
                {
                    continue;
                }
                hasColorProperty |= material.HasProperty("_BaseColor") || material.HasProperty("_Color");
            }
            if (!hasColorProperty)
            {
                continue;
            }

            currentRenderer.GetPropertyBlock(block);
            block.SetColor("_BaseColor", tint);
            block.SetColor("_Color", tint);
            currentRenderer.SetPropertyBlock(block);
        }
    }
}
