using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OSIGame.SessionLayer
{
    public class CheckpointShift : ShiftBase
    {
        [Header("Rules")]
        [SerializeField] float fileMB = 100f;
        [SerializeField] float rateMBps = 5f;
        [SerializeField] float checkpointCost = 0.7f;
        [SerializeField] float reconnectTime = 1.5f;
        [SerializeField] float timeLimit = 60f;

        [Header("Progress bar")]
        [SerializeField] RectTransform barArea;           // the bar's background
        [SerializeField] RectTransform progressFill;      // child of barArea
        [SerializeField] Image progressFillImage;
        [SerializeField] Image lostOverlay;               // child of barArea, red
        [SerializeField] RectTransform checkpointMarker;  // inactive template, child of barArea

        [Header("Texts")]
        [SerializeField] TMP_Text sizeText;
        [SerializeField] TMP_Text statusText;
        [SerializeField] TMP_Text statsText;
        [SerializeField] TMP_Text timeText;
        [SerializeField] RectTransform timerFill;

        [Header("Transfer controls")]
        [SerializeField] GameObject transferControls;
        [SerializeField] Button checkpointButton;

        [Header("Ending the session")]
        [SerializeField] GameObject closeChoices;
        [SerializeField] Button gracefulButton;
        [SerializeField] Button abortButton;
        [SerializeField] Button leaveOpenButton;
        [SerializeField] GameObject explanation;
        [SerializeField] TMP_Text explanationText;
        [SerializeField] Button continueButton;

        const int TransferMax = 80, CloseMax = 20;

        enum Phase { Transfer, Close, Explained }

        Phase _phase;
        float _progress, _lastCheckpoint, _elapsed;
        float _pauseTimer, _nextCrash;
        bool _reconnecting, _finished;
        string _pauseReason = "";
        int _crashes, _transferScore;
        float _resentMB, _lostTimer;
        readonly List<GameObject> _markers = new List<GameObject>();

        void Reset()
        {
            title = "Shift 3: Checkpoint It";
            concept = "Long transfers get interrupted. The session layer adds <b>synchronization points</b> (checkpoints) to the " +
                      "data stream. If the connection drops, the session is <b>resynchronized</b> to the last checkpoint both sides " +
                      "agreed on, instead of starting over from zero.\n\nWhen the conversation is done, the session layer also handles " +
                      "<b>termination</b>: closing the session so both sides know it is over.";
            howToPlay = "Send a 100 MB file within 60 seconds over a flaky link. Click <b>Insert checkpoint</b> to save a sync point. " +
                        "Each one pauses the transfer for 0.7 s, so too many waste time, but too few means losing lots of progress " +
                        "when the link drops. When the transfer is done, decide how to end the session.";
            lesson = "Checkpoints trade a small, steady cost for protection against a big loss: after a failure, only the data since " +
                     "the last sync point is resent. Sessions should end with an orderly release (both sides confirm) rather than " +
                     "an abrupt abort, so no data still in flight is lost.";
        }

        void Awake()
        {
            checkpointButton.onClick.AddListener(InsertCheckpoint);
            gracefulButton.onClick.AddListener(() => Choose(0));
            abortButton.onClick.AddListener(() => Choose(1));
            leaveOpenButton.onClick.AddListener(() => Choose(2));
            continueButton.onClick.AddListener(Complete);
            checkpointMarker.gameObject.SetActive(false);
        }

        protected override void OnBeginShift()
        {
            _phase = Phase.Transfer;
            _progress = _lastCheckpoint = _elapsed = 0f;
            _pauseTimer = 0f;
            _reconnecting = _finished = false;
            _nextCrash = Random.Range(3.5f, 6f);
            _crashes = 0;
            _resentMB = 0f;
            _lostTimer = 0f;
            foreach (var m in _markers) Destroy(m);
            _markers.Clear();
            ShowPhase();
            Refresh();
        }

        void Update()
        {
            float dt = Time.deltaTime;
            if (_lostTimer > 0f) _lostTimer -= dt;

            if (_phase == Phase.Transfer)
            {
                _elapsed += dt;
                if (_pauseTimer > 0f)
                {
                    _pauseTimer -= dt;
                    if (_pauseTimer <= 0f) _reconnecting = false;
                }
                else
                {
                    _progress += rateMBps * dt;
                    if (_progress >= fileMB)
                    {
                        _progress = fileMB;
                        EndTransfer(true);
                    }
                    else
                    {
                        // Link drops are timed against active transfer time.
                        _nextCrash -= dt;
                        if (_nextCrash <= 0f && _progress < fileMB - 2f) Crash();
                    }
                }
                if (_phase == Phase.Transfer && _elapsed >= timeLimit) EndTransfer(false);
            }

            Refresh();
        }

        void Crash()
        {
            _crashes++;
            float lost = _progress - _lastCheckpoint;
            _resentMB += lost;

            // Show the lost section of the bar in red for a moment.
            var rt = lostOverlay.rectTransform;
            rt.anchorMin = new Vector2(_lastCheckpoint / fileMB, 0f);
            rt.anchorMax = new Vector2(_progress / fileMB, 1f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            _lostTimer = 1.5f;

            _progress = _lastCheckpoint;
            _pauseTimer = reconnectTime;
            _reconnecting = true;
            _pauseReason = "Link dropped! Resynchronizing to checkpoint at " + _lastCheckpoint.ToString("0.0") + " MB ...";
            _nextCrash = Random.Range(4f, 8f);

            if (_lastCheckpoint <= 0f)
                ShowFeedback("Connection lost with NO checkpoint. The transfer restarts from 0 MB (" + lost.ToString("0.0") + " MB wasted).", Bad);
            else
                ShowFeedback("Connection lost. Resumed from " + _lastCheckpoint.ToString("0.0") + " MB, so only " + lost.ToString("0.0") + " MB must be resent.", Warn);
        }

        void InsertCheckpoint()
        {
            if (_phase != Phase.Transfer || _pauseTimer > 0f) return;
            if (_progress - _lastCheckpoint < 1f)
            {
                ShowFeedback("You just saved a checkpoint here.", Muted, 2f);
                return;
            }
            _lastCheckpoint = _progress;
            _pauseTimer = checkpointCost;
            _pauseReason = "Both sides acknowledging sync point at " + _progress.ToString("0.0") + " MB ...";

            // Place a flag on the bar at this point.
            var marker = Instantiate(checkpointMarker, barArea);
            marker.gameObject.SetActive(true);
            marker.anchorMin = marker.anchorMax = new Vector2(_progress / fileMB, 0.5f);
            marker.anchoredPosition = Vector2.zero;
            _markers.Add(marker.gameObject);
        }

        void EndTransfer(bool finished)
        {
            _finished = finished;
            _phase = Phase.Close;
            _pauseTimer = 0f;
            _reconnecting = false;
            _transferScore = finished
                ? Mathf.RoundToInt(Mathf.Clamp(TransferMax - Mathf.Max(0f, _elapsed - 36f) * 2f, 30f, TransferMax))
                : Mathf.RoundToInt(_progress / fileMB * 40f);
            ShowPhase();
        }

        void Choose(int choice)
        {
            if (_phase != Phase.Close) return;
            _phase = Phase.Explained;
            Score = Mathf.Clamp(_transferScore + (choice == 0 ? CloseMax : 0), 0, MaxScore);
            ResultLine = (_finished ? "Transfer finished in " + _elapsed.ToString("0.0") + "s"
                                    : "Transfer reached " + _progress.ToString("0") + " MB before time ran out") +
                         " with " + _markers.Count + " checkpoints, " + _crashes + " drops and " + _resentMB.ToString("0") +
                         " MB resent. " + (choice == 0 ? "Session closed gracefully." : "Session not closed properly.");

            if (choice == 0)
            {
                explanationText.text = "Correct! An orderly release lets both sides confirm everything arrived before the session ends.";
                explanationText.color = Good;
            }
            else if (choice == 1)
            {
                explanationText.text = "An abort ends the session abruptly: data still in flight may be lost, and the peer can't be sure the last data arrived.";
                explanationText.color = Bad;
            }
            else
            {
                explanationText.text = "Idle sessions tie up memory and resources on both sides until a timeout finally kills them. Close sessions you're done with.";
                explanationText.color = Bad;
            }
            ShowPhase();
        }

        void ShowPhase()
        {
            transferControls.SetActive(_phase == Phase.Transfer);
            closeChoices.SetActive(_phase == Phase.Close);
            explanation.SetActive(_phase == Phase.Explained);
        }

        void Refresh()
        {
            statsText.text = "Checkpoints " + Col(_markers.Count.ToString(), Accent) +
                             "     Link drops " + Col(_crashes.ToString(), _crashes > 0 ? Bad : Color.white) +
                             "     Data resent " + Col(_resentMB.ToString("0.0") + " MB", _resentMB > 0f ? Warn : Color.white);
            timeText.text = "Time " + _elapsed.ToString("0.0") + " / " + timeLimit.ToString("0") + "s";
            SetBar(timerFill, 1f - _elapsed / timeLimit);

            sizeText.text = _progress.ToString("0.0") + " / " + fileMB.ToString("0") + " MB";
            SetBar(progressFill, _progress / fileMB);
            if (progressFillImage) progressFillImage.color = _reconnecting ? Warn : Good;

            var c = lostOverlay.color;
            c.a = Mathf.Clamp01(_lostTimer / 1.5f);
            lostOverlay.color = c;

            checkpointButton.interactable = _phase == Phase.Transfer && _pauseTimer <= 0f;

            if (_phase != Phase.Transfer)
                statusText.text = _finished ? Col("Transfer complete. The session is still open. How should it end?", Good)
                                            : Col("Out of time; the transfer didn't finish. The session is still open. How should it end?", Bad);
            else if (_pauseTimer > 0f)
                statusText.text = Col(_pauseReason, _reconnecting ? Bad : Accent);
            else
                statusText.text = "Transferring at " + rateMBps + " MB/s ...   last checkpoint: " +
                                  (_lastCheckpoint > 0f ? _lastCheckpoint.ToString("0.0") + " MB" : Col("none", Bad));
        }
    }
}