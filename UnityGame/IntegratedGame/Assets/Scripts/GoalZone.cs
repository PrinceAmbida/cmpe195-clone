using UnityEngine;

public class GoalZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // must complete all 3 routers first
            if (NetworkGameManager.instance.lastRouterReached >= 3)
            {
                NetworkGameManager.instance.LevelComplete();
            }
        }
    }
}
