using UnityEngine;

public class SegmentCollectible : MonoBehaviour
{
    public float rotateSpeed = 50f;
    public bool isTCP = true; // true = TCP segment, false = UDP segment

    void Update()
    {
        // Spin so the player can see it
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TCP segments add score, UDP segments add score but no order check
            GameManager.instance.CollectSegment(isTCP);
            Destroy(gameObject);
        }
    }
}
