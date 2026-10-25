using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OSIGame.SessionLayer
{
    public abstract class ShiftBase : MonoBehaviour
    {
        [Header("Text shown before and after this shift")]
        public string title = "Shift";
        [TextArea(4, 10)] public string concept;
        [TextArea(3, 8)] public string howToPlay;
        [TextArea(3, 8)] public string lesson;

        [Header("Feedback banner")]
        [SerializeField] protected TMP_Text feedbackText;

        public static readonly Color Good = new Color(0.40f, 0.85f, 0.50f);
        public static readonly Color Bad = new Color(0.95f, 0.40f, 0.40f);
        public static readonly Color Warn = new Color(0.98f, 0.78f, 0.30f);
        public static readonly Color Accent = new Color(0.35f, 0.75f, 0.95f);
        public static readonly Color Muted = new Color(0.60f, 0.66f, 0.75f);
        public static readonly Color Track = new Color(0.18f, 0.21f, 0.28f);

        public virtual int MaxScore { get { return 100; } }
        public int Score { get; protected set; }
        public string ResultLine { get; protected set; }
        public event System.Action Completed;

        Coroutine _feedbackRoutine;

        public void ResetScore()
        {
            Score = 0;
            ResultLine = "";
        }

        public void BeginShift()
        {
            ResetScore();
            if (feedbackText) feedbackText.text = "";
            OnBeginShift();
        }

        protected abstract void OnBeginShift();

        protected void Complete()
        {
            if (Completed != null) Completed();
        }

        protected void ShowFeedback(string message, Color color, float seconds = 4f)
        {
            if (!feedbackText) return;
            if (_feedbackRoutine != null) StopCoroutine(_feedbackRoutine);
            feedbackText.text = message;
            feedbackText.color = color;
            _feedbackRoutine = StartCoroutine(ClearFeedbackAfter(seconds));
        }

        IEnumerator ClearFeedbackAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            feedbackText.text = "";
            _feedbackRoutine = null;
        }

        // Stretch a "fill" child across fraction t of its parent (no sprite needed).
        protected static void SetBar(RectTransform fill, float t)
        {
            if (!fill) return;
            fill.anchorMin = new Vector2(0f, 0f);
            fill.anchorMax = new Vector2(Mathf.Clamp01(t), 1f);
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
        }

        protected static string Col(string s, Color c)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGB(c) + ">" + s + "</color>";
        }

        protected static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}