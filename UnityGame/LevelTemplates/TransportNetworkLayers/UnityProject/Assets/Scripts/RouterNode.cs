using UnityEngine;

public class RouterNode : MonoBehaviour
{
    public int routerNumber = 1;
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            // strict order: must be exactly the next one
            if (routerNumber == NetworkGameManager.instance.lastRouterReached + 1)
            {
                activated = true;
                NetworkGameManager.instance.ReachRouter(routerNumber);
                Renderer routerRenderer = GetComponent<Renderer>();
                if (routerRenderer != null)
                {
                    MaterialPropertyBlock colorBlock = new MaterialPropertyBlock();
                    routerRenderer.GetPropertyBlock(colorBlock);
                    colorBlock.SetColor("_BaseColor", Color.cyan);
                    colorBlock.SetColor("_Color", Color.cyan);
                    routerRenderer.SetPropertyBlock(colorBlock);
                }
            }
            else
            {
                NetworkGameManager.instance.ReportWrongRouter(routerNumber);
            }
        }
    }
}
