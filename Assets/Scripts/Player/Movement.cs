using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private CharacterController playerCharController;
    private bool isGrounded;

    private Vector3 movementDirection;
    private Vector2 lookInput;
    private float verticalVelocity;
    public float speed = 5f;
    public float jumpForce = 1.5f;
    public float gravity = -9.8f;

    public float lookSensitivity = 100f; // Mouse sensitivity for look rotation
    private float pitch = 0f;

    public Transform cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCharController = GetComponent<CharacterController>();
        if (cameraTransform != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = playerCharController.isGrounded;

        MovePlayer();
        Applylook();

    }
    void Applylook()
    {
        float yaw = lookInput.x * lookSensitivity * Time.deltaTime;
        transform.Rotate(0, yaw, 0);

        if (cameraTransform != null)
        {
            pitch -= lookInput.y * lookSensitivity * Time.deltaTime; // Subtract for natural mouse direction
            pitch = Mathf.Clamp(pitch, -80f, 80f); // Clamp to avoid flipping
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }
    }

    void MovePlayer()
    {
        verticalVelocity += gravity * Time.deltaTime;

        if (playerCharController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.1f; // Small negative to keep controller grounded
        }

        Vector3 worldMovementDirection = transform.TransformDirection(movementDirection);
        worldMovementDirection.y = verticalVelocity;
        playerCharController.Move(worldMovementDirection * speed * Time.deltaTime);
        Debug.Log(movementDirection);
    }

    public void HandleMove(InputAction.CallbackContext context)
    {


        Vector3 input = context.ReadValue<Vector2>();
        movementDirection = new Vector3(input.x, 0, input.y);

    }
    public void HandleJump(InputAction.CallbackContext context)
    {

        if (!context.performed || !isGrounded) return;
        verticalVelocity = jumpForce;

    }

    public void HandleLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

}
