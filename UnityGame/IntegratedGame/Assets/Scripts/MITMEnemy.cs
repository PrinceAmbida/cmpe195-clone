using UnityEngine;

public class MITMEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float moveRange = 3f;
    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * direction * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) >= moveRange)
        {
            direction *= -1;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // send player back to last router checkpoint
            NetworkGameManager.instance.InterceptPlayer(collision.gameObject);
        }
    }
}
