using UnityEngine;

// Application-layer protocols the player can route requests to.
public enum AppProtocol { HTTP, HTTPS, DNS, SMTP, IMAP, FTP, SSH }

// One "client request" the player has to route to the right server.
// Create via: Assets > Create > OSI > Application Layer Request
[CreateAssetMenu(fileName = "NewRequest", menuName = "OSI/Application Layer Request")]
public class AppLayerRequest : ScriptableObject
{
    [Tooltip("What the user/app wants, in plain language. Shown on the packet.")]
    [TextArea] public string clientMessage = "I want to check my inbox";

    [Tooltip("The protocol/server that should handle this request.")]
    public AppProtocol correctProtocol = AppProtocol.IMAP;

    [Tooltip("If true, the player must take the packet to the DNS server first to resolve the hostname.")]
    public bool requiresDnsLookup;

    [Tooltip("Hostname shown when DNS resolves it (only used if requiresDnsLookup).")]
    public string hostname = "mail.school.edu";

    [Tooltip("Teaching text shown after the request is handled (or listed for review if the player got it wrong).")]
    [TextArea(3, 6)] public string explanation =
        "IMAP (port 143 / 993 with TLS) lets an email client read messages that stay stored on the mail server.";
}
