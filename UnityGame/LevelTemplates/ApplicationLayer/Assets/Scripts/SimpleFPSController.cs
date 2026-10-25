using UnityEngine;

// Minimal first-person controller. Swap for your team's shared controller if you have one.
[RequireComponent(typeof(CharacterController))]
public class SimpleFPSController : MonoBehaviour
{
    [SerializeField] Transform cameraPivot;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lookSensitivity = 2f;
    [SerializeField] float gravity = -20f;

    CharacterController cc;
    float pitch, verticalVel;

    void Awake() => cc = GetComponent<CharacterController>();

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void Update()
    {
        var gm = AppLayerGameManager.Instance;
        if (gm && !gm.IsRunning) return;

        if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked;
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Look
        transform.Rotate(0f, Input.GetAxis("Mouse X") * lookSensitivity, 0f);
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * lookSensitivity, -85f, 85f);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

        // Move
        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        if (cc.isGrounded && verticalVel < 0f) verticalVel = -2f;
        verticalVel += gravity * Time.deltaTime;
        cc.Move((Vector3.ClampMagnitude(move, 1f) * moveSpeed + Vector3.up * verticalVel) * Time.deltaTime);
    }
}
