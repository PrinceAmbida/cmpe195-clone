using System.Collections;
using TMPro;
using UnityEngine;

// A server "building"/terminal in the level that handles one application-layer protocol.
public class ProtocolServer : MonoBehaviour
{
    public AppProtocol protocol;
    [Tooltip("Well-known port shown on the sign, e.g. HTTP 80, HTTPS 443, DNS 53, SMTP 25/587, IMAP 143/993, FTP 21, SSH 22")]
    public string port = "80";

    [SerializeField] TMP_Text signLabel;
    [SerializeField] Renderer statusLight;
    [SerializeField] Color idleColor = Color.gray;

    void Start()
    {
        if (signLabel) signLabel.text = $"{protocol}\n<size=60%>port {port}</size>";
        SetLight(idleColor);
    }

    // Returns true if the packet was consumed (player no longer holds it).
    public bool Receive(RequestPacket packet) =>
        AppLayerGameManager.Instance.HandleDelivery(this, packet);

    public void Flash(Color c) => StartCoroutine(FlashRoutine(c));

    IEnumerator FlashRoutine(Color c)
    {
        SetLight(c);
        yield return new WaitForSeconds(1f);
        SetLight(idleColor);
    }

    void SetLight(Color c)
    {
        if (statusLight) statusLight.material.color = c;
    }
}
