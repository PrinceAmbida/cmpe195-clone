using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OSIGame.SessionLayer
{
    public class QuizShift : ShiftBase
    {
        [System.Serializable]
        public class Question
        {
            [TextArea(2, 4)] public string text;
            public string[] options = new string[4];
            [Tooltip("0 = first option")] public int correct;
            [TextArea(2, 4)] public string why;

            public Question(string text, string[] options, int correct, string why)
            {
                this.text = text; this.options = options; this.correct = correct; this.why = why;
            }
        }

        [Header("Questions")]
        [SerializeField] List<Question> questions = new List<Question>();

        [Header("UI")]
        [SerializeField] TMP_Text counterText;
        [SerializeField] TMP_Text questionText;
        [SerializeField] Button[] optionButtons = new Button[4];
        [SerializeField] TMP_Text verdictText;
        [SerializeField] TMP_Text whyText;
        [SerializeField] Button nextButton;

        int _index, _picked, _correctCount;
        Color[] _normalColors;

        void Reset()
        {
            title = "Shift 4: Debrief";
            concept = "Time to check what you learned. The session layer:\n\n" +
                      "- <b>establishes</b> sessions (request, authenticate, negotiate, confirm with a session ID)\n" +
                      "- <b>manages the dialog</b> (simplex, half-duplex with a token, full-duplex)\n" +
                      "- <b>synchronizes</b> with checkpoints so failures don't mean starting over\n" +
                      "- <b>terminates</b> sessions in an orderly way";
            howToPlay = "Five questions. Pick the best answer; an explanation appears after each one.";
            lesson = "The session layer is the conversation manager: it opens, controls, checkpoints and closes the dialog " +
                     "between two applications, while the layers below it just move the bits.";

            questions = new List<Question>
            {
                new Question("Where does the session layer sit in the OSI model?",
                    new[] { "Layer 4, below Transport", "Layer 5, between Transport and Presentation",
                            "Layer 6, between Presentation and Application", "Layer 3, alongside Network" }, 1,
                    "Session is Layer 5. It sits on top of Transport (4), which moves the bytes, and below Presentation (6), which formats them."),
                new Question("In half-duplex dialog control, how does the session layer stop both sides from talking at once?",
                    new[] { "Only the side holding the token may transmit", "Both transmit and the stronger signal wins",
                            "It adds MAC addresses to each frame", "It encrypts every message" }, 0,
                    "A token grants the right to send. Passing it hands the turn to the other side."),
                new Question("A 500 MB transfer has sync points every 50 MB. The link drops at 430 MB. Where does it resume?",
                    new[] { "0 MB: start over", "430 MB: exactly where it dropped",
                            "400 MB: the last checkpoint", "450 MB: the next checkpoint" }, 2,
                    "Both sides only agreed on data up to the last sync point (400 MB), so that is where the session resynchronizes."),
                new Question("Why does the server give each session its own ID?",
                    new[] { "To pick the fastest route across the internet", "To compress the data",
                            "To convert data into a common format",
                            "To keep each conversation's exchanges separate, even between the same two hosts" }, 3,
                    "The session ID tags every exchange so many sessions can run side by side. Routing is Layer 3; compression and formatting are Layer 6."),
                new Question("What is the advantage of a graceful release over just dropping the connection?",
                    new[] { "It's faster", "Both sides confirm they're done, so no data in flight is lost",
                            "It skips authentication next time", "It upgrades the session to full-duplex" }, 1,
                    "An orderly release is a two-way agreement to stop, so neither side is left wondering if the last data arrived."),
            };
        }

        void Awake()
        {
            _normalColors = new Color[optionButtons.Length];
            for (int i = 0; i < optionButtons.Length; i++)
            {
                int index = i;
                _normalColors[i] = optionButtons[i].image.color;
                optionButtons[i].onClick.AddListener(() => Pick(index));
            }
            nextButton.onClick.AddListener(Next);
        }

        protected override void OnBeginShift()
        {
            _index = 0;
            _correctCount = 0;
            ShowQuestion();
        }

        void ShowQuestion()
        {
            var q = questions[_index];
            _picked = -1;
            counterText.text = "QUESTION " + (_index + 1) + " / " + questions.Count;
            questionText.text = q.text;
            for (int i = 0; i < optionButtons.Length; i++)
            {
                bool used = i < q.options.Length;
                optionButtons[i].gameObject.SetActive(used);
                if (!used) continue;
                optionButtons[i].image.color = _normalColors[i];
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = q.options[i];
            }
            verdictText.text = "";
            whyText.text = "";
            nextButton.gameObject.SetActive(false);
        }

        void Pick(int i)
        {
            if (_picked >= 0) return; // already answered
            var q = questions[_index];
            _picked = i;
            bool right = i == q.correct;
            if (right) _correctCount++;

            optionButtons[q.correct].image.color = Good;
            if (!right) optionButtons[i].image.color = Bad;

            verdictText.text = right ? "Correct!" : "Not quite.";
            verdictText.color = right ? Good : Bad;
            whyText.text = q.why;

            bool last = _index == questions.Count - 1;
            nextButton.GetComponentInChildren<TMP_Text>().text = last ? "Finish" : "Next question";
            nextButton.gameObject.SetActive(true);
        }

        void Next()
        {
            if (_index == questions.Count - 1)
            {
                Score = _correctCount * MaxScore / questions.Count;
                ResultLine = _correctCount + " of " + questions.Count + " answers correct.";
                Complete();
            }
            else
            {
                _index++;
                ShowQuestion();
            }
        }
    }
}