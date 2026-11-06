using TMPro;
using UnityEngine;

// Physical "request" object the player picks up and delivers.
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class RequestPacket : MonoBehaviour
{
    [SerializeField] Renderer body;
    [SerializeField] TMP_Text floatingLabel;   // optional world-space label above the packet
    [SerializeField] Color needsDnsColor = new Color(1f, 0.8f, 0.2f);
    [SerializeField] Color readyColor = new Color(0.3f, 0.9f, 1f);

    public AppLayerRequest Request { get; private set; }
    public bool DnsResolved { get; private set; }
    public string ResolvedIp { get; private set; }

    Rigidbody rb;
    Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void Init(AppLayerRequest request)
    {
        Request = request;
        DnsResolved = !request.requiresDnsLookup;
        Refresh();
    }

    public void MarkDnsResolved()
    {
        DnsResolved = true;
        ResolvedIp = $"93.184.{Random.Range(1, 255)}.{Random.Range(1, 255)}";
        Refresh();
    }

    public void SetHeld(bool held, Transform holdPoint)
    {
        rb.isKinematic = held;
        col.enabled = !held;
        transform.SetParent(held ? holdPoint : null);
        if (held)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public string Summary =>
        DnsResolved ? Request.clientMessage
                    : $"{Request.clientMessage}\n<size=70%>(needs DNS lookup: {Request.hostname})</size>";

    void Refresh()
    {
        if (body) body.material.color = DnsResolved ? readyColor : needsDnsColor;
        if (floatingLabel) floatingLabel.text = Summary;
    }

    void LateUpdate()
    {
        // Keep the floating label facing the camera.
        if (floatingLabel && Camera.main)
            floatingLabel.transform.rotation = Camera.main.transform.rotation;
    }
}
