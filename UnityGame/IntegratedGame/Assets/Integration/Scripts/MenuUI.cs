// Inside the Network - integration layer
// Small helper that builds uGUI elements from code, so the menu scenes need only one
// GameObject each and nothing in them can lose a reference when files move between projects.
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InsideTheNetwork.Integration
{
    public static class MenuUI
    {
        // Palette taken from the Transport/Network result panels so the app feels like one game.
        public static readonly Color Background = new Color32(9, 14, 28, 255);
        public static readonly Color Panel = new Color32(19, 28, 50, 240);
        public static readonly Color Accent = new Color32(91, 224, 255, 255);
        public static readonly Color Gold = new Color32(255, 211, 112, 255);
        public static readonly Color TextColor = new Color32(230, 238, 250, 255);
        public static readonly Color Muted = new Color32(150, 165, 190, 255);
        public static readonly Color Blue = new Color32(30, 103, 146, 255);
        public static readonly Color Teal = new Color32(28, 126, 108, 255);
        public static readonly Color Purple = new Color32(81, 74, 131, 255);
        public static readonly Color Grey = new Color32(48, 55, 72, 255);

        public static readonly Vector2 ReferenceResolution = new Vector2(1200, 700);

        static Font font;
        public static Font Font
        {
            get
            {
                if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return font;
            }
        }

        public static void EnsureCameraAndEventSystem()
        {
            if (Camera.main == null)
            {
                GameObject go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                go.tag = "MainCamera";
                Camera cam = go.GetComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Background;
                cam.orthographic = true;
            }
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                // Active Input Handling is "Both", so the classic module works everywhere.
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }

        public static RectTransform CreateCanvas(string name, int sortingOrder = 0)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            CanvasScaler scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;
            return (RectTransform)go.transform;
        }

        public static RectTransform Rect(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // anchor = point on the parent (0..1), pos = offset from it, size in reference pixels
        public static void Place(RectTransform rt, Vector2 anchor, Vector2 size, Vector2 pos)
        {
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
        }

        public static Image Box(Transform parent, string name, Color color)
        {
            RectTransform rt = Rect(parent, name);
            Image image = rt.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text Label(Transform parent, string text, int size, Color color,
            TextAnchor align = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            RectTransform rt = Rect(parent, "Label");
            Text label = rt.gameObject.AddComponent<Text>();
            label.font = Font;
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = align;
            label.fontStyle = style;
            label.supportRichText = true;
            label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            return label;
        }

        public static Button Button(Transform parent, string title, Color color, UnityAction onClick, int fontSize = 22)
        {
            Image image = Box(parent, title + "Button", color);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f);
            colors.pressedColor = new Color(0.72f, 0.72f, 0.72f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
            button.colors = colors;
            if (onClick != null) button.onClick.AddListener(onClick);

            Text label = Label(image.transform, title, fontSize, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
            Stretch(label.rectTransform);
            return button;
        }
    }
}
