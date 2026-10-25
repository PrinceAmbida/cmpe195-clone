using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OSIGame.SessionLayer
{
    public class TokenShift : ShiftBase
    {
        [Header("Rules")]
        [SerializeField] float duration = 50f;
        [SerializeField] int goal = 15;
        [SerializeField] float travelTime = 0.7f;
        [SerializeField] float tokenPassTime = 0.4f;

        [Header("Hosts (index 0 = Alice, 1 = Bob)")]
        [SerializeField] RectTransform aliceHost;
        [SerializeField] RectTransform bobHost;
        [SerializeField] GameObject aliceTokenRing;
        [SerializeField] GameObject bobTokenRing;
        [SerializeField] RectTransform aliceTokenSpot;
        [SerializeField] RectTransform bobTokenSpot;
        [SerializeField] Image[] aliceSlots;
        [SerializeField] Image[] bobSlots;
        [SerializeField] TMP_Text aliceOutboxText;
        [SerializeField] TMP_Text bobOutboxText;

        [Header("Moving pieces")]
        [SerializeField] Image messageDot;
        [SerializeField] RectTransform tokenDot;

        [Header("Status")]
        [SerializeField] TMP_Text statsText;
        [SerializeField] RectTransform timerFill;
        [SerializeField] Image timerFillImage;

        [Header("Buttons")]
        [SerializeField] Button sendAliceButton;
        [SerializeField] Button sendBobButton;
        [SerializeField] Button passTokenButton;

        static readonly string[] Names = { "ALICE", "BOB" };

        readonly int[] _queue = new int[2];
        int _holder, _lastSpawnSide;
        float _timeLeft, _spawnTimer;

        bool _inFlight, _garbled;
        int _flightFrom;
        float _flightT;

        bool _tokenMoving;
        float _tokenT;

        int _delivered, _collisions, _dropped;
        float _endTimer = -1f;

        int QueueCap { get { return aliceSlots.Length; } }

        void Reset()
        {
            title = "Shift 2: Pass the Token";
            concept = "The session layer handles <b>dialog control</b>: who may talk, and when.\n\n" +
                      "<b>Simplex</b>: only one side ever sends.\n<b>Half-duplex</b>: both can send, but one at a time.\n" +
                      "<b>Full-duplex</b>: both send at the same time.\n\n" +
                      "In half-duplex, a <b>token</b> decides whose turn it is. Only the side holding it may transmit; " +
                      "speaking out of turn causes a collision and the message must be resent.";
            howToPlay = "Alice and Bob share a half-duplex session and messages pile up in their outboxes.\n\n" +
                        "Deliver <b>15 messages</b> in 50 seconds. Send from the token holder, and <b>pass the token</b> when the " +
                        "other side has a backlog. Sending without the token causes a collision; a full outbox drops messages.";
            lesson = "Half-duplex dialog control is turn-taking enforced by a token. Whoever holds it talks; handing it over gives " +
                     "the other side the floor. Good token management avoids collisions without starving either side. " +
                     "(Full-duplex sessions don't need a token at all.)";
        }

        void Awake()
        {
            sendAliceButton.onClick.AddListener(() => Send(0));
            sendBobButton.onClick.AddListener(() => Send(1));
            passTokenButton.onClick.AddListener(PassToken);
        }

        protected override void OnBeginShift()
        {
            _queue[0] = 2;
            _queue[1] = 0;
            _holder = 0;
            _lastSpawnSide = 1;
            _timeLeft = duration;
            _spawnTimer = 1.2f;
            _inFlight = false;
            _tokenMoving = false;
            _delivered = _collisions = _dropped = 0;
            _endTimer = -1f;
            SetButtons(true);
            Refresh();
        }

        void Update()
        {
            float dt = Time.deltaTime;

            if (_endTimer >= 0f)
            {
                _endTimer -= dt;
                if (_endTimer < 0f) Finish();
                return;
            }

            _timeLeft -= dt;

            // New outgoing messages arrive in bursts, usually on the same side as the last one.
            _spawnTimer -= dt;
            if (_spawnTimer <= 0f)
            {
                _spawnTimer = Random.Range(0.9f, 1.8f);
                int side = Random.value < 0.7f ? _lastSpawnSide : 1 - _lastSpawnSide;
                _lastSpawnSide = side;
                if (_queue[side] >= QueueCap)
                {
                    _dropped++;
                    ShowFeedback(Names[side] + "'s outbox overflowed and a message was dropped! Give the token to whoever has a backlog.", Warn, 3f);
                }
                else _queue[side]++;
            }

            if (_inFlight)
            {
                _flightT += dt / travelTime;
                if (_flightT >= 1f)
                {
                    _inFlight = false;
                    if (!_garbled)
                    {
                        _queue[_flightFrom]--;
                        _delivered++;
                    }
                }
            }

            if (_tokenMoving)
            {
                _tokenT += dt / tokenPassTime;
                if (_tokenT >= 1f)
                {
                    _tokenMoving = false;
                    _holder = 1 - _holder;
                }
            }

            if (_delivered >= goal || _timeLeft <= 0f)
            {
                _timeLeft = Mathf.Max(0f, _timeLeft);
                _endTimer = 1.2f;
                SetButtons(false);
                if (_delivered >= goal) ShowFeedback("All " + goal + " messages delivered!", Good, 1.2f);
                else ShowFeedback("Time's up!", Warn, 1.2f);
            }

            Refresh();
        }

        void Finish()
        {
            int d = Mathf.Min(_delivered, goal);
            Score = Mathf.Clamp(d * 100 / goal - _collisions * 10 - _dropped * 5, 0, 100);
            ResultLine = "Delivered " + d + "/" + goal + ", " + _collisions + " collision" + (_collisions == 1 ? "" : "s") +
                         ", " + _dropped + " dropped.";
            Complete();
        }

        void Send(int side)
        {
            if (_endTimer >= 0f) return;
            if (_queue[side] == 0) { ShowFeedback(Names[side] + " has nothing to send.", Muted, 2f); return; }
            if (_inFlight) { ShowFeedback("The line is busy. Wait for the current message to arrive.", Muted, 2f); return; }
            if (_tokenMoving) { ShowFeedback("The token is being handed over. Wait a moment.", Muted, 2f); return; }

            _inFlight = true;
            _flightFrom = side;
            _flightT = 0f;
            _garbled = side != _holder;
            if (_garbled)
            {
                _collisions++;
                ShowFeedback(Names[side] + " talked without the token and collided with " + Names[1 - side] +
                             "'s turn! The message was garbled and must be resent.", Bad);
            }
        }

        void PassToken()
        {
            if (_endTimer >= 0f || _tokenMoving) return;
            if (_inFlight) { ShowFeedback("Let the current message finish before handing over the token.", Muted, 2f); return; }
            _tokenMoving = true;
            _tokenT = 0f;
        }

        void SetButtons(bool on)
        {
            sendAliceButton.interactable = on;
            sendBobButton.interactable = on;
            passTokenButton.interactable = on;
        }

        void Refresh()
        {
            // Status line and timer bar
            statsText.text = "Delivered " + Col(_delivered + "/" + goal, Good) +
                             "     Collisions " + Col(_collisions.ToString(), _collisions > 0 ? Bad : Color.white) +
                             "     Dropped " + Col(_dropped.ToString(), _dropped > 0 ? Warn : Color.white);
            SetBar(timerFill, _timeLeft / duration);
            if (timerFillImage) timerFillImage.color = _timeLeft < 10f ? Bad : Accent;

            // Who holds the token
            aliceTokenRing.SetActive(!_tokenMoving && _holder == 0);
            bobTokenRing.SetActive(!_tokenMoving && _holder == 1);
            tokenDot.gameObject.SetActive(_tokenMoving);
            if (_tokenMoving)
            {
                Vector3 from = _holder == 0 ? aliceTokenSpot.position : bobTokenSpot.position;
                Vector3 to = _holder == 0 ? bobTokenSpot.position : aliceTokenSpot.position;
                tokenDot.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, _tokenT));
            }
            passTokenButton.GetComponentInChildren<TMP_Text>().text = "Pass token " + (_holder == 0 ? "->" : "<-");

            // Outboxes
            DrawSlots(aliceSlots, _queue[0]);
            DrawSlots(bobSlots, _queue[1]);
            aliceOutboxText.text = "outbox " + _queue[0] + "/" + QueueCap;
            bobOutboxText.text = "outbox " + _queue[1] + "/" + QueueCap;

            // Message in flight: blue if delivered, red and bursting if it collided
            messageDot.gameObject.SetActive(_inFlight);
            if (_inFlight)
            {
                Vector3 from = _flightFrom == 0 ? aliceHost.position : bobHost.position;
                Vector3 to = _flightFrom == 0 ? bobHost.position : aliceHost.position;
                float t = _garbled ? Mathf.Min(_flightT * 2f, 1f) * 0.5f : _flightT;
                messageDot.rectTransform.position = Vector3.Lerp(from, to, t);
                messageDot.color = _garbled ? Bad : Accent;
                float scale = _garbled ? 1f + Mathf.Max(0f, _flightT - 0.5f) * 3f : 1f;
                messageDot.rectTransform.localScale = Vector3.one * scale;
            }
        }

        void DrawSlots(Image[] slots, int count)
        {
            for (int i = 0; i < slots.Length; i++)
                slots[i].color = i < count ? (count >= slots.Length ? Bad : Accent) : Track;
        }
    }
}