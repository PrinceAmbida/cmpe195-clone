using UnityEngine;
using UnityEngine.UI;

public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager instance;

    public Text statusText;
    public Text timerText;
    public GameObject factPanel;
    public Text factText;
    private Text routeHintText;

    public float levelTime = 90f;

    // respawn points for each router — drag the router positions here
    public Transform startPoint;
    public Transform router1Point;
    public Transform router2Point;
    public Transform router3Point;

    public int lastRouterReached = 0; // 0 = start, 1, 2, 3
    private float timeLeft;
    private bool levelDone = false;
    private float routeHintTimeLeft;
    private string routeHintMessage;

    private string[] facts = {
        "A Man-in-the-Middle attack intercepts communication between two parties without their knowledge.",
        "HTTPS encrypts your data so even if intercepted, the attacker cannot read it.",
        "ARP spoofing is a common MITM technique where an attacker links their MAC address to a legitimate IP.",
        "The Network Layer uses IP addresses and routing to find the best path between devices.",
        "VPNs protect against MITM attacks by encrypting all traffic between you and the server.",
        "Routers at the Network Layer forward packets toward their destination using routing tables."
    };

    void Awake()
    {
        instance = this;
        LayerBackgroundMusic.Play(transform, LayerBackgroundMusic.NetworkTrack);
    }

    void Start()
    {
        Time.timeScale = 1f;
        timeLeft = levelTime;
        factPanel.SetActive(false);
        GameObject routeHintObject = GameObject.Find("ProtocolText");
        if (routeHintObject != null)
        {
            routeHintText = routeHintObject.GetComponent<Text>();
            FirstPersonHud.StyleSceneCenterCounter(routeHintText);
        }
        FirstPersonHud.StyleSceneCounter(statusText, false);
        FirstPersonHud.StyleSceneCounter(timerText, true);
        FirstPersonHud.StyleResultPanel(factPanel, factText);
        UpdateUI();
    }

    void Update()
    {
        if (routeHintTimeLeft > 0f)
        {
            routeHintTimeLeft -= Time.unscaledDeltaTime;
            if (routeHintTimeLeft <= 0f)
            {
                routeHintMessage = null;
                UpdateRouteHint();
            }
        }

        if (levelDone) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            LevelFailed();
        }

        UpdateUI();
    }

    public void ReachRouter(int routerNumber)
    {
        if (routerNumber > lastRouterReached)
        {
            lastRouterReached = routerNumber;
            ShowRouteHint("CHECKPOINT SAVED: ROUTER " + routerNumber);
            UpdateUI();
        }
    }

    public void ReportWrongRouter(int routerNumber)
    {
        int nextRouter = lastRouterReached + 1;
        if (routerNumber > nextRouter)
        {
            ShowRouteHint("REACH ROUTER " + nextRouter + " FIRST");
        }
    }

    public void InterceptPlayer(GameObject player)
    {
        // send player back to last router reached
        PlayerRespawn respawn = player.GetComponent<PlayerRespawn>();
        if (respawn == null) return;

        string checkpoint = lastRouterReached == 0 ? "START" : "ROUTER " + lastRouterReached;
        ShowRouteHint("MITM INTERCEPT! BACK TO " + checkpoint);

        switch (lastRouterReached)
        {
            case 0: respawn.RespawnAt(startPoint); break;
            case 1: respawn.RespawnAt(router1Point); break;
            case 2: respawn.RespawnAt(router2Point); break;
            case 3: respawn.RespawnAt(router3Point); break;
        }
    }

    public void LevelComplete()
    {
        if (levelDone) return;
        levelDone = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int index = Random.Range(0, facts.Length);
        factText.text = "Packet Delivered!\n\n" + facts[index];
        factPanel.SetActive(true);
    }

    void LevelFailed()
    {
        levelDone = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        factText.text = "Connection Timed Out!\n\nThe packet never reached the server. Try again!";
        factPanel.SetActive(true);
    }

    void UpdateUI()
    {
        statusText.text = "Routers: " + lastRouterReached + " / 3";
        timerText.text = "Time: " + Mathf.CeilToInt(timeLeft) + "s";
        UpdateRouteHint();
    }

    private void ShowRouteHint(string message)
    {
        routeHintMessage = message;
        routeHintTimeLeft = 2.5f;
        UpdateRouteHint();
    }

    private void UpdateRouteHint()
    {
        if (routeHintText == null)
        {
            return;
        }

        if (routeHintTimeLeft > 0f && !string.IsNullOrEmpty(routeHintMessage))
        {
            routeHintText.text = routeHintMessage;
            return;
        }

        routeHintText.text = lastRouterReached >= 3
            ? "ALL ROUTERS REACHED - DELIVER PACKET"
            : "NEXT: ROUTER " + (lastRouterReached + 1);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
