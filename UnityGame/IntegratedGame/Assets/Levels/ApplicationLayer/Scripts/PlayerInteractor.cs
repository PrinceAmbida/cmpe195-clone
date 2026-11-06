using TMPro;
using UnityEngine;

// Put on the player. Raycasts from the camera to pick up packets and deliver them to servers.
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform holdPoint;          // empty child in front of the camera
    [SerializeField] float reach = 3.5f;
    [SerializeField] KeyCode interactKey = KeyCode.E;
    [SerializeField] TMP_Text promptText;          // small text near the crosshair
    [SerializeField] TMP_Text carryingText;        // HUD: what request you're holding

    RequestPacket held;

    void Update()
    {
        if (AppLayerGameManager.Instance && !AppLayerGameManager.Instance.IsRunning)
        {
            SetPrompt("");
            return;
        }

        RaycastHit hit;
        bool hasHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, reach);
        Debug.Log($"Ray from {cam.name} at {cam.transform.position} | hit: {(hasHit ? hit.collider.name : "nothing")}");
        var server = hasHit ? hit.collider.GetComponentInParent<ProtocolServer>() : null;
        var packet = hasHit ? hit.collider.GetComponentInParent<RequestPacket>() : null;

        if (held == null)
        {
            SetPrompt(packet ? $"[{interactKey}] Pick up request" : "");
            if (packet && Input.GetKeyDown(interactKey)) PickUp(packet);
        }
        else if (server)
        {
            SetPrompt($"[{interactKey}] Send to {server.protocol} server");
            if (Input.GetKeyDown(interactKey) && server.Receive(held)) held = null;
        }
        else
        {
            SetPrompt($"[{interactKey}] Drop");
            if (Input.GetKeyDown(interactKey)) Drop();
        }

        if (carryingText)
            carryingText.text = held ? $"Carrying: {held.Summary}" : "";
    }

    void PickUp(RequestPacket p)
    {
        held = p;
        held.SetHeld(true, holdPoint);
    }

    void Drop()
    {
        held.SetHeld(false, null);
        held = null;
    }

    void SetPrompt(string s)
    {
        if (promptText) promptText.text = s;
    }
}
