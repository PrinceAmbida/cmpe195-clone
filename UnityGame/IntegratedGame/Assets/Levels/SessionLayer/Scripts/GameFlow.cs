using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OSIGame.SessionLayer
{
    public class GameFlow : MonoBehaviour
    {
        [Header("Screens (panels under the Canvas)")]
        [SerializeField] GameObject titleScreen;
        [SerializeField] GameObject introScreen;
        [SerializeField] GameObject resultScreen;
        [SerializeField] GameObject finalScreen;

        [Header("Shifts, in play order")]
        [SerializeField] ShiftBase[] shifts;

        [Header("Header bar")]
        [SerializeField] TMP_Text scoreText;
        [SerializeField] Image[] progressPips;

        [Header("Title screen")]
        [SerializeField] Button startButton;

        [Header("Intro screen")]
        [SerializeField] TMP_Text introTitle;
        [SerializeField] TMP_Text introConcept;
        [SerializeField] TMP_Text introHowToPlay;
        [SerializeField] Button beginButton;

        [Header("Result screen")]
        [SerializeField] TMP_Text resultTitle;
        [SerializeField] TMP_Text resultScore;
        [SerializeField] TMP_Text resultLine;
        [SerializeField] TMP_Text resultLesson;
        [SerializeField] Button nextButton;

        [Header("Final screen")]
        [SerializeField] TMP_Text finalScore;
        [SerializeField] TMP_Text finalBreakdown;
        [SerializeField] Button playAgainButton;
        [SerializeField] Button backToMenuButton;

        [Header("Events for your hub / level select")]
        public UnityEvent<int, int> onFinished;   // (score, maxScore)
        public UnityEvent onBackToMenu;

        int _index;
        bool _currentDone;

        void Awake()
        {
            startButton.onClick.AddListener(StartGame);
            beginButton.onClick.AddListener(BeginCurrentShift);
            nextButton.onClick.AddListener(Next);
            playAgainButton.onClick.AddListener(StartGame);
            if (backToMenuButton) backToMenuButton.onClick.AddListener(() => onBackToMenu.Invoke());
            foreach (var s in shifts) s.Completed += OnShiftCompleted;
        }

        void Start()
        {
            ShowOnly(titleScreen);
            UpdateHeader();
        }

        int TotalScore
        {
            get { int t = 0; foreach (var s in shifts) t += s.Score; return t; }
        }

        int MaxScore
        {
            get { int t = 0; foreach (var s in shifts) t += s.MaxScore; return t; }
        }

        void ShowOnly(GameObject screen)
        {
            titleScreen.SetActive(screen == titleScreen);
            introScreen.SetActive(screen == introScreen);
            resultScreen.SetActive(screen == resultScreen);
            finalScreen.SetActive(screen == finalScreen);
            foreach (var s in shifts) s.gameObject.SetActive(s.gameObject == screen);
        }

        void StartGame()
        {
            if (shifts == null || shifts.Length == 0)
            {
                Debug.LogWarning("GameFlow: add at least one shift to the Shifts list.");
                return;
            }
            foreach (var s in shifts) s.ResetScore();
            _index = 0;
            ShowIntro();
        }

        void ShowIntro()
        {
            var s = shifts[_index];
            _currentDone = false;
            introTitle.text = s.title;
            introConcept.text = s.concept;
            introHowToPlay.text = s.howToPlay;
            ShowOnly(introScreen);
            UpdateHeader();
        }

        void BeginCurrentShift()
        {
            var s = shifts[_index];
            ShowOnly(s.gameObject);   // turn the panel on first, then start it
            s.BeginShift();
        }

        void OnShiftCompleted()
        {
            var s = shifts[_index];
            _currentDone = true;
            float pct = (float)s.Score / s.MaxScore;
            resultTitle.text = s.title + "  -  complete";
            resultScore.text = s.Score + " / " + s.MaxScore;
            resultScore.color = pct >= 0.8f ? ShiftBase.Good : pct >= 0.5f ? ShiftBase.Warn : ShiftBase.Bad;
            resultLine.text = s.ResultLine;
            resultLesson.text = s.lesson;
            nextButton.GetComponentInChildren<TMP_Text>().text = _index == shifts.Length - 1 ? "See final score" : "Next shift";
            ShowOnly(resultScreen);
            UpdateHeader();
        }

        void Next()
        {
            if (_index == shifts.Length - 1)
            {
                ShowFinal();
            }
            else
            {
                _index++;
                ShowIntro();
            }
        }

        void ShowFinal()
        {
            int total = TotalScore, max = MaxScore;
            float pct = max > 0 ? (float)total / max : 0f;
            string rank = pct >= 0.9f ? "Session Master" : pct >= 0.7f ? "Stable Connection"
                        : pct >= 0.5f ? "Intermittent Link" : "Session Timed Out";
            finalScore.text = total + " / " + max + "   " + rank;
            finalScore.color = pct >= 0.7f ? ShiftBase.Good : pct >= 0.5f ? ShiftBase.Warn : ShiftBase.Bad;

            var sb = new System.Text.StringBuilder();
            foreach (var s in shifts)
                sb.Append("<b>").Append(s.title).Append("</b>   ").Append(s.Score).Append(" / ").Append(s.MaxScore)
                  .Append("\n<size=80%><color=#99A8BF>").Append(s.ResultLine).Append("</color></size>\n\n");
            finalBreakdown.text = sb.ToString();

            ShowOnly(finalScreen);
            UpdateHeader();
            onFinished.Invoke(total, max);
        }

        void UpdateHeader()
        {
            if (scoreText) scoreText.text = "Score " + TotalScore;
            bool onTitle = titleScreen.activeSelf;
            bool onFinal = finalScreen.activeSelf;
            for (int i = 0; i < progressPips.Length; i++)
            {
                Color c = ShiftBase.Track;
                if (!onTitle)
                {
                    if (onFinal || i < _index || (i == _index && _currentDone)) c = ShiftBase.Good;
                    else if (i == _index) c = ShiftBase.Accent;
                }
                progressPips[i].color = c;
            }
        }
    }
}