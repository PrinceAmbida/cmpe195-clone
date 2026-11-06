using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -10f;
    private Vector3 startPos;
    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        transform.position = startPos;
        rb.velocity = Vector3.zero;
    }

    // respawn at a specific point (used by NetworkGameManager)
    public void RespawnAt(Transform point)
    {
        transform.position = point.position;
        rb.velocity = Vector3.zero;
    }
}
