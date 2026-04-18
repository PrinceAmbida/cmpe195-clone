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
        // Respawn if player falls off the map
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        transform.position = startPos;
        rb.linearVelocity = Vector3.zero;
    }
}
