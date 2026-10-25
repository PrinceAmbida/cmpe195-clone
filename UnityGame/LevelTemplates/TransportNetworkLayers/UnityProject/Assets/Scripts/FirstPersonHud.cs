using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class FirstPersonHud
{
    public static void StyleSceneCounter(UnityEngine.UI.Text counter, bool rightAligned)
    {
        if (counter == null)
        {
            return;
        }

        CanvasScaler sceneScaler = counter.GetComponentInParent<CanvasScaler>();
        if (sceneScaler != null)
        {
            sceneScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sceneScaler.referenceResolution = new Vector2(1200f, 700f);
            sceneScaler.matchWidthOrHeight = 0.5f;
        }

        counter.fontSize = 28;
        counter.fontStyle = FontStyle.Bold;
        counter.color = new Color32(239, 248, 255, 255);
        counter.alignment = rightAligned ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
        counter.horizontalOverflow = HorizontalWrapMode.Overflow;
        counter.verticalOverflow = VerticalWrapMode.Overflow;
        counter.raycastTarget = false;

        RectTransform rect = counter.rectTransform;
        Vector2 corner = rightAligned ? Vector2.one : new Vector2(0f, 1f);
        rect.anchorMin = corner;
        rect.anchorMax = corner;
        rect.pivot = corner;
        rect.anchoredPosition = rightAligned ? new Vector2(-28f, -16f) : new Vector2(28f, -16f);
        rect.sizeDelta = new Vector2(280f, 48f);

        UnityEngine.UI.Outline outline = counter.GetComponent<UnityEngine.UI.Outline>();
        if (outline == null)
        {
            outline = counter.gameObject.AddComponent<UnityEngine.UI.Outline>();
        }
        outline.effectColor = new Color32(3, 12, 27, 240);
        outline.effectDistance = new Vector2(2f, -2f);
    }

    public static void StyleSceneCenterCounter(UnityEngine.UI.Text counter)
    {
        StyleSceneCounter(counter, false);
        if (counter == null)
        {
            return;
        }

        counter.fontSize = 24;
        counter.alignment = TextAnchor.MiddleCenter;
        RectTransform rect = counter.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -24f);
        rect.sizeDelta = new Vector2(460f, 48f);
    }

    public static void StyleResultPanel(GameObject panel, UnityEngine.UI.Text factText)
    {
        if (panel == null || factText == null)
        {
            return;
        }

        UnityEngine.UI.Image backdrop = panel.GetComponent<UnityEngine.UI.Image>();
        if (backdrop != null)
        {
            backdrop.color = new Color32(6, 15, 30, 245);
        }

        factText.color = new Color32(245, 250, 255, 255);
        factText.fontSize = 28;
        factText.alignment = TextAnchor.MiddleCenter;
        factText.horizontalOverflow = HorizontalWrapMode.Wrap;
        factText.verticalOverflow = VerticalWrapMode.Overflow;
        factText.raycastTarget = false;
        RectTransform factRect = factText.rectTransform;
        factRect.anchorMin = new Vector2(0.14f, 0.5f);
        factRect.anchorMax = new Vector2(0.86f, 0.5f);
        factRect.pivot = new Vector2(0.5f, 0.5f);
        factRect.anchoredPosition = new Vector2(0f, 55f);
        factRect.sizeDelta = new Vector2(0f, 230f);

    }

    public static void Create(string sceneName)
    {
        if (GameObject.Find("FirstPersonHUD") != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject(
            "FirstPersonHUD",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1200f, 700f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panelObject = new GameObject("ObjectiveAndControls", typeof(RectTransform), typeof(Image));
        panelObject.transform.SetParent(canvasObject.transform, false);
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.zero;
        panelRect.pivot = Vector2.zero;
        panelRect.anchoredPosition = new Vector2(28f, 20f);
        panelRect.sizeDelta = new Vector2(1144f, 130f);

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0.015f, 0.045f, 0.09f, 0.86f);
        panelImage.raycastTarget = false;

        GameObject textObject = new GameObject("ObjectiveText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(panelObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(24f, 14f);
        textRect.offsetMax = new Vector2(-24f, -14f);

        TextMeshProUGUI objectiveText = textObject.GetComponent<TextMeshProUGUI>();
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (font != null)
        {
            objectiveText.font = font;
        }
        objectiveText.fontSize = 20f;
        objectiveText.color = new Color32(232, 244, 255, 255);
        objectiveText.richText = true;
        objectiveText.enableWordWrapping = true;
        objectiveText.alignment = TextAlignmentOptions.TopLeft;
        objectiveText.raycastTarget = false;

        bool isNetworkLayer = sceneName == "NetworkLayer";
        string layerName = isNetworkLayer ? "NETWORK LAYER" : "TRANSPORT LAYER";
        string titleColor = isNetworkLayer ? "#81E6FF" : "#FFD58A";
        string objective = isNetworkLayer
            ? "You are a data packet. Reach Routers 1, 2 and 3, then the destination. Avoid MITM attackers."
            : "You are a data packet. Collect 3 TCP segments and 2 UDP datagrams. Avoid DoS attackers.";
        objectiveText.text = "<size=125%><color=" + titleColor + "><b>" + layerName + "</b></color></size>\n" +
                             "<color=#68DCFF><b>OBJECTIVE</b></color>  " + objective +
                             "\n<size=90%><color=#B7D2EA>WASD MOVE  |  MOUSE LOOK  |  SPACE JUMP  |  ESC RELEASE MOUSE  |  TAB LOCK MOUSE</color></size>";

        GameObject reticleObject = new GameObject("Reticle", typeof(RectTransform), typeof(TextMeshProUGUI));
        reticleObject.transform.SetParent(canvasObject.transform, false);
        RectTransform reticleRect = reticleObject.GetComponent<RectTransform>();
        reticleRect.anchorMin = new Vector2(0.5f, 0.5f);
        reticleRect.anchorMax = new Vector2(0.5f, 0.5f);
        reticleRect.pivot = new Vector2(0.5f, 0.5f);
        reticleRect.anchoredPosition = Vector2.zero;
        reticleRect.sizeDelta = new Vector2(36f, 36f);

        TextMeshProUGUI reticle = reticleObject.GetComponent<TextMeshProUGUI>();
        if (font != null)
        {
            reticle.font = font;
        }
        reticle.text = "+";
        reticle.fontSize = 20f;
        reticle.fontStyle = FontStyles.Bold;
        reticle.color = new Color32(115, 234, 255, 230);
        reticle.alignment = TextAlignmentOptions.Center;
        reticle.raycastTarget = false;
    }
}
