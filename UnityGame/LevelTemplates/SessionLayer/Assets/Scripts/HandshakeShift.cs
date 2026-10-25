using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OSIGame.SessionLayer
{
    public class HandshakeShift : ShiftBase
    {
        enum Step { Request, Authenticate, Negotiate, Confirm, Exchange, Reject }

        class Client
        {
            public string Name, App;
            public bool ValidCreds;
            public Client(string name, string app, bool valid) { Name = name; App = app; ValidCreds = valid; }
        }

        [Header("Client card")]
        [SerializeField] TMP_Text clientCounterText;
        [SerializeField] TMP_Text clientNameText;
        [SerializeField] TMP_Text appText;
        [SerializeField] TMP_Text credentialsText;
        [SerializeField] TMP_Text sessionIdText;
        [SerializeField] TMP_Text stateText;
        [SerializeField] TMP_Text mistakesText;

        [Header("Session log")]
        [SerializeField] TMP_Text logText;

        [Header("The 6 step buttons (any order)")]
        [SerializeField] Button[] stepButtons = new Button[6];

        static readonly Step[] ValidPath = { Step.Request, Step.Authenticate, Step.Negotiate, Step.Confirm, Step.Exchange };
        static readonly Step[] InvalidPath = { Step.Request, Step.Authenticate, Step.Reject };

        readonly List<Client> _clients = new List<Client>();
        readonly List<Step> _buttonSteps = new List<Step>();
        readonly List<string> _log = new List<string>();
        Step[] _expected;
        int _clientIndex, _progress, _mistakes;
        float _advanceTimer = -1f;
        string _sessionId;

        // Fills in the teaching text when you add the component in the editor.
        void Reset()
        {
            title = "Shift 1: Open the Session";
            concept = "Before two applications can talk, the <b>session layer</b> sets up a <b>session</b>: a managed conversation " +
                      "with a beginning, a middle and an end.\n\nThe client <b>requests</b> a session, the server <b>authenticates</b> it, " +
                      "both sides <b>negotiate</b> the rules, the server <b>confirms</b> and issues a <b>session ID</b>, and only then " +
                      "does <b>data exchange</b> begin.";
            howToPlay = "Clients connect one at a time. Click the steps in the correct order to set up each session.\n\n" +
                        "Some clients will fail authentication. A session must never continue past a failed check, so <b>reject</b> " +
                        "those. Each wrong click costs points.";
            lesson = "A session is set up deliberately: request, authenticate, negotiate, confirm (session ID), exchange data. " +
                     "The session ID keeps many conversations apart, and a failed authentication ends things before any data flows.";
        }

        void Awake()
        {
            for (int i = 0; i < stepButtons.Length; i++)
            {
                int index = i; // captured copy for the lambda
                stepButtons[i].onClick.AddListener(() => OnStepButton(index));
            }
        }

        protected override void OnBeginShift()
        {
            _clients.Clear();
            _clients.Add(new Client("laptop-07", "Video call", true));
            _clients.Add(new Client("kiosk-12", "Remote desktop", false));
            _clients.Add(new Client("phone-33", "File sync", true));
            _clients.Add(new Client("unknown-host", "Admin console", false));
            _clients.Add(new Client("db-replica", "Database replication", true));
            Shuffle(_clients);
            _mistakes = 0;
            _advanceTimer = -1f;
            StartClient(0);
        }

        void StartClient(int index)
        {
            _clientIndex = index;
            var c = _clients[index];
            _expected = c.ValidCreds ? ValidPath : InvalidPath;
            _progress = 0;
            _sessionId = Random.Range(0x1000, 0xFFFF).ToString("X4");

            // Shuffle which button shows which step.
            _buttonSteps.Clear();
            _buttonSteps.AddRange(new[] { Step.Request, Step.Authenticate, Step.Negotiate, Step.Confirm, Step.Exchange, Step.Reject });
            Shuffle(_buttonSteps);
            for (int i = 0; i < stepButtons.Length; i++)
            {
                stepButtons[i].GetComponentInChildren<TMP_Text>().text = Label(_buttonSteps[i]);
                stepButtons[i].interactable = true;
            }

            _log.Clear();
            _log.Add(Col("... " + c.Name + " is knocking ...", Muted));
            Refresh();
        }

        void Update()
        {
            if (_advanceTimer < 0f) return;
            _advanceTimer -= Time.deltaTime;
            if (_advanceTimer >= 0f) return;
            _advanceTimer = -1f;

            if (_clientIndex + 1 < _clients.Count)
            {
                StartClient(_clientIndex + 1);
            }
            else
            {
                Score = Mathf.Max(0, 100 - 8 * _mistakes);
                ResultLine = "Handled " + _clients.Count + " clients with " + _mistakes + " mistake" + (_mistakes == 1 ? "" : "s") + ".";
                Complete();
            }
        }

        void OnStepButton(int buttonIndex)
        {
            if (_advanceTimer >= 0f) return;
            Step clicked = _buttonSteps[buttonIndex];
            var c = _clients[_clientIndex];
            Step want = _expected[_progress];

            if (clicked == want)
            {
                _log.Add(LogLine(clicked, c));
                _progress++;
                if (_progress == _expected.Length)
                {
                    _advanceTimer = 1.6f;
                    foreach (var b in stepButtons) b.interactable = false;
                    if (c.ValidCreds) ShowFeedback("Session " + _sessionId + " is open with " + c.Name + ".", Good, 1.6f);
                    else ShowFeedback("Refused " + c.Name + ". Nothing was exchanged with an unauthenticated client.", Good, 1.6f);
                }
                else if (clicked == Step.Authenticate && !c.ValidCreds)
                {
                    ShowFeedback("Authentication FAILED for " + c.Name + ". What now?", Warn, 3f);
                }
            }
            else
            {
                _mistakes++;
                ShowFeedback(MistakeText(clicked, want, c), Bad);
            }
            Refresh();
        }

        string MistakeText(Step clicked, Step want, Client c)
        {
            bool authenticated = _progress >= 2;
            if (clicked == Step.Reject)
                return authenticated ? "Authentication succeeded, so there's no reason to refuse this session."
                                     : "You can't judge a client before checking its credentials. Authenticate first.";
            if (!c.ValidCreds && authenticated)
                return "Authentication failed! A session must not continue past a failed check. Reject it.";
            return "Out of order. " + Hint(want);
        }

        static string Hint(Step s)
        {
            switch (s)
            {
                case Step.Request: return "Nothing happens until the client's request is acknowledged.";
                case Step.Authenticate: return "Verify who is asking before agreeing to anything.";
                case Step.Negotiate: return "Both sides must agree on the dialog rules before the session is confirmed.";
                case Step.Confirm: return "The session must be confirmed and given an ID before data can flow.";
                case Step.Exchange: return "The session is ready. Let the data flow.";
                default: return "";
            }
        }

        static string Label(Step s)
        {
            switch (s)
            {
                case Step.Request: return "Acknowledge request";
                case Step.Authenticate: return "Authenticate client";
                case Step.Negotiate: return "Negotiate rules";
                case Step.Confirm: return "Confirm + assign session ID";
                case Step.Exchange: return "Start data exchange";
                default: return "Reject session";
            }
        }

        string LogLine(Step s, Client c)
        {
            switch (s)
            {
                case Step.Request: return c.Name + " -> server : SESSION REQUEST (" + c.App + ")";
                case Step.Authenticate: return "server : checking credentials ... " + (c.ValidCreds ? Col("OK", Good) : Col("FAILED", Bad));
                case Step.Negotiate: return "both : agree on half-duplex + checkpoint every 10 MB";
                case Step.Confirm: return "server -> " + c.Name + " : SESSION ACCEPT, id=" + Col(_sessionId, Accent);
                case Step.Exchange: return "[" + _sessionId + "] data exchange started";
                default: return "server -> " + c.Name + " : " + Col("SESSION REFUSED", Bad);
            }
        }

        string StateLabel(Client c)
        {
            if (_progress == 0) return Col("WAITING", Muted);
            switch (_expected[_progress - 1])
            {
                case Step.Request: return Col("REQUESTED", Warn);
                case Step.Authenticate: return c.ValidCreds ? Col("AUTHENTICATED", Accent) : Col("AUTH FAILED", Bad);
                case Step.Negotiate: return Col("RULES AGREED", Accent);
                case Step.Confirm: return Col("CONFIRMED", Good);
                case Step.Exchange: return Col("OPEN - EXCHANGING DATA", Good);
                default: return Col("REFUSED", Bad);
            }
        }

        void Refresh()
        {
            var c = _clients[_clientIndex];
            bool authDone = _progress >= 2;
            bool idIssued = c.ValidCreds && _progress >= 4;

            clientCounterText.text = "INCOMING CLIENT  " + (_clientIndex + 1) + " / " + _clients.Count;
            clientNameText.text = c.Name;
            appText.text = "Application:  <b>" + c.App + "</b>";
            credentialsText.text = "Credentials:  " + (!authDone ? Col("not checked yet", Muted)
                : c.ValidCreds ? Col("valid", Good) : Col("INVALID / EXPIRED", Bad));
            sessionIdText.text = "Session ID:  " + (idIssued ? Col(_sessionId, Accent) : Col("none", Muted));
            stateText.text = "State:  " + StateLabel(c);
            mistakesText.text = "Mistakes: " + _mistakes;
            mistakesText.color = _mistakes > 0 ? Bad : Muted;

            int first = Mathf.Max(0, _log.Count - 7);
            logText.text = string.Join("\n", _log.GetRange(first, _log.Count - first));
        }
    }
}