// Padmavathy Elencheliyan
// Inside the Network
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera positioning above player
public class FixedCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    public float fixedX = 0f;
    public float fixedY = 10f;

    void LateUpdate()
    {
        transform.position = new Vector3(
            fixedX,
            fixedY,
            player.position.z
        );
    }
}
