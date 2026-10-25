using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float mouseSensitivity = 2f;
    public float cameraHeight = 0.62f;
    public float firstPersonFieldOfView = 80f;

    private Rigidbody rb;
    private Transform cameraTransform;
    private Vector2 moveInput;
    private float yaw;
    private float pitch;
    private bool isGrounded;
    private bool cursorLocked;
    private bool firstPersonMode;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        string sceneName = SceneManager.GetActiveScene().name;
        firstPersonMode = sceneName == "TransportLayer" || sceneName == "NetworkLayer";
        yaw = transform.eulerAngles.y;
        if (firstPersonMode)
        {
            rb.constraints |= RigidbodyConstraints.FreezeRotation;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            // Both courses run from left to right along the X axis.
            yaw = 90f;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                cameraTransform.SetParent(transform, false);
                cameraTransform.localPosition = Vector3.up * cameraHeight;
                cameraTransform.localRotation = Quaternion.identity;
                mainCamera.orthographic = false;
                mainCamera.fieldOfView = firstPersonFieldOfView;
                mainCamera.nearClipPlane = 0.05f;
                mainCamera.farClipPlane = 150f;
            }

            Renderer playerRenderer = GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                // Keep the collider and Rigidbody while hiding the sphere from its own camera.
                playerRenderer.enabled = false;
            }

            SetCursorLocked(true);
            WorldObjectLabeler.CreateLabels(mainCamera);
            FirstPersonHud.Create(sceneName);
        }
    }

    void Update()
    {
        if (Time.timeScale <= 0f)
        {
            moveInput = Vector2.zero;
            return;
        }

        if (firstPersonMode && Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLocked(false);
        }
        else if (firstPersonMode && Input.GetKeyDown(KeyCode.Tab))
        {
            SetCursorLocked(true);
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        else if (firstPersonMode && !cursorLocked && Input.GetMouseButtonDown(0))
        {
            // Browsers require a click or key press before locking the mouse.
            SetCursorLocked(true);
        }
#endif

        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        if (firstPersonMode && cameraTransform != null && cursorLocked)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, -78f, 78f);

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        Vector3 movement = firstPersonMode
            ? (transform.right * moveInput.x + transform.forward * moveInput.y) * moveSpeed
            : new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        Vector3 velocity = rb.velocity;
        velocity.x = movement.x;
        velocity.z = movement.z;
        rb.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnDisable()
    {
        if (firstPersonMode)
        {
            SetCursorLocked(false);
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && firstPersonMode)
        {
            SetCursorLocked(false);
        }
    }

    private void SetCursorLocked(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
