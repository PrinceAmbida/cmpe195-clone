using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Runs the Application Layer minigame: spawns client requests, judges deliveries, tracks score.
public class AppLayerGameManager : MonoBehaviour
{
    public static AppLayerGameManager Instance { get; private set; }

    [Header("Requests")]
    [SerializeField] List<AppLayerRequest> requestPool = new();
    [SerializeField] RequestPacket packetPrefab;
    [SerializeField] Transform[] spawnPoints;       // e.g. on the "Client PC" desk
    [SerializeField] int maxActivePackets = 3;
    [SerializeField] float spawnInterval = 6f;

    [Header("Rules")]
    [SerializeField] float roundLength = 180f;
    [SerializeField] int pointsCorrect = 100;
    [SerializeField] int pointsWrongServer = -50;
    [SerializeField] int pointsSkippedDns = -25;
    [SerializeField] int passScore = 800;

    [Header("UI")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] float feedbackDuration = 6f;
    [SerializeField] GameObject summaryPanel;
    [SerializeField] TMP_Text summaryText;

    [Header("Hook into the main OSI game")]
    public UnityEvent<bool> onMinigameComplete;     // passes true if the player hit passScore

    public bool IsRunning { get; private set; }

    readonly List<RequestPacket> active = new();
    readonly HashSet<AppLayerRequest> missed = new();
    int score, delivered, mistakes;
    float timeLeft, spawnTimer;
    Coroutine feedbackRoutine;

    void Awake() => Instance = this;

    void Start() => StartRound();

    public void StartRound()
    {
        foreach (var p in active) if (p) Destroy(p.gameObject);
        active.Clear();
        missed.Clear();
        score = delivered = mistakes = 0;
        timeLeft = roundLength;
        spawnTimer = 0f;
        IsRunning = true;
        if (summaryPanel) summaryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        ShowFeedback("Route each client request to the right application-layer server!");
    }

    void Update()
    {
        if (!IsRunning) return;

        timeLeft -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;
        active.RemoveAll(p => p == null);

        if (spawnTimer <= 0f && active.Count < maxActivePackets)
        {
            SpawnPacket();
            spawnTimer = spawnInterval;
        }

        if (scoreText) scoreText.text = $"Score: {score}";
        if (timerText) timerText.text = $"{Mathf.CeilToInt(Mathf.Max(timeLeft, 0f))}s";

        if (timeLeft <= 0f) EndRound();
    }

    void SpawnPacket()
    {
        if (requestPool.Count == 0 || spawnPoints.Length == 0) return;
        var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        var packet = Instantiate(packetPrefab, point.position + Vector3.up * 0.5f, Quaternion.identity);
        packet.Init(requestPool[Random.Range(0, requestPool.Count)]);
        active.Add(packet);
    }

    // Called by ProtocolServer. Returns true if the packet was consumed.
    public bool HandleDelivery(ProtocolServer server, RequestPacket packet)
    {
        var req = packet.Request;

        // Step 1: DNS lookup for requests that use a hostname.
        if (server.protocol == AppProtocol.DNS && !packet.DnsResolved)
        {
            packet.MarkDnsResolved();
            server.Flash(Color.cyan);
            ShowFeedback($"DNS resolved <b>{req.hostname}</b> → {packet.ResolvedIp}\n" +
                         "Now the client knows which IP address to contact. Deliver it to the right server!");
            return false; // player keeps carrying it
        }

        // Right server, but the client never looked up the IP.
        if (server.protocol == req.correctProtocol && !packet.DnsResolved)
        {
            AddScore(pointsSkippedDns, req, server);
            ShowFeedback($"<color=#ffb347>Unreachable!</color> The client only knows the name <b>{req.hostname}</b>, " +
                         "not an IP address. Resolve it at the DNS server first.");
            return false;
        }

        // Correct delivery.
        if (server.protocol == req.correctProtocol)
        {
            score += pointsCorrect;
            delivered++;
            server.Flash(Color.green);
            ShowFeedback($"<color=#7CFC00>Correct!</color> {req.explanation}");
            active.Remove(packet);
            Destroy(packet.gameObject);
            return true;
        }

        // Wrong server.
        AddScore(pointsWrongServer, req, server);
        ShowFeedback($"<color=#ff6b6b>Wrong server.</color> A {server.protocol} server can't handle " +
                     $"\"{req.clientMessage}\". Which protocol is built for that?");
        return false;
    }

    void AddScore(int penalty, AppLayerRequest req, ProtocolServer server)
    {
        score += penalty;
        mistakes++;
        missed.Add(req);
        server.Flash(Color.red);
    }

    void ShowFeedback(string msg)
    {
        if (!feedbackText) return;
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(FeedbackRoutine(msg));
    }

    IEnumerator FeedbackRoutine(string msg)
    {
        feedbackText.text = msg;
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.text = "";
    }

    void EndRound()
    {
        IsRunning = false;
        foreach (var p in active) if (p) Destroy(p.gameObject);
        active.Clear();
        Cursor.lockState = CursorLockMode.None;

        bool passed = score >= passScore;
        var sb = new StringBuilder();
        sb.AppendLine(passed ? "<b>Application Layer: PASSED</b>" : "<b>Application Layer: Try Again</b>");
        sb.AppendLine($"Score: {score}   Delivered: {delivered}   Mistakes: {mistakes}\n");
        if (missed.Count > 0)
        {
            sb.AppendLine("<b>Review these:</b>");
            foreach (var r in missed) sb.AppendLine($"• {r.clientMessage} → <b>{r.correctProtocol}</b>: {r.explanation}");
        }
        else sb.AppendLine("Perfect routing — no mistakes!");

        if (summaryPanel) summaryPanel.SetActive(true);
        if (summaryText) summaryText.text = sb.ToString();
        onMinigameComplete?.Invoke(passed);
    }
}
