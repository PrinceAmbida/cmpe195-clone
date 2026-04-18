using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text scoreText;
    public Text timerText;
    public Text protocolText;
    public GameObject factPanel;
    public Text factText;

    public float levelTime = 60f;
    public int totalSegments = 5;

    private int tcpCollected = 0;
    private int udpCollected = 0;
    private float timeLeft;
    private bool levelDone = false;

    // Transport Layer networking facts
    private string[] facts = {
        "TCP uses a three-way handshake (SYN, SYN-ACK, ACK) to establish a connection.",
        "UDP is faster than TCP because it does not check if data arrives correctly.",
        "TCP guarantees data arrives in order. UDP does not.",
        "A DoS attack floods the Transport Layer with fake connection requests to crash a server.",
        "Port numbers at the Transport Layer identify which application should receive the data.",
        "TCP breaks data into segments and numbers them so the receiver can reassemble in order.",
        "Online games often use UDP because speed matters more than perfect delivery."
    };

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timeLeft = levelTime;
        factPanel.SetActive(false);
        UpdateUI();
    }

    void Update()
    {
        if (levelDone) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            LevelFailed();
        }

        UpdateUI();
    }

    public void CollectSegment(bool isTCP)
    {
        if (isTCP)
            tcpCollected++;
        else
            udpCollected++;

        UpdateUI();

        int total = tcpCollected + udpCollected;
        if (total >= totalSegments)
        {
            LevelComplete();
        }
    }

    void UpdateUI()
    {
        int total = tcpCollected + udpCollected;
        scoreText.text = "Segments: " + total + " / " + totalSegments;
        timerText.text = "Time: " + Mathf.CeilToInt(timeLeft) + "s";
        protocolText.text = "TCP: " + tcpCollected + "  UDP: " + udpCollected;
    }

    void LevelComplete()
    {
        levelDone = true;
        Time.timeScale = 0f;

        int index = Random.Range(0, facts.Length);
        factText.text = "Level Complete!\n\n" + facts[index];
        factPanel.SetActive(true);
    }

    void LevelFailed()
    {
        levelDone = true;
        Time.timeScale = 0f;

        factText.text = "Time's Up!\n\nThe segments were lost in transit. Try again!";
        factPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
