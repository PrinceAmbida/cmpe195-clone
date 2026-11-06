using UnityEngine;

public class DoSEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float moveRange = 4f;

    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Enemy moves back and forth to block the player
        transform.position += Vector3.right * moveSpeed * direction * Time.deltaTime;

        // Reverse direction when reaching move range
        if (Vector3.Distance(startPos, transform.position) >= moveRange)
        {
            direction *= -1;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Push player back to start, simulating a DoS attack
            PlayerRespawn respawn = collision.gameObject.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.Respawn();
            }
        }
    }
}
