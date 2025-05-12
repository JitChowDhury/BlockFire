using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private CharacterController playerCharController;
    private bool isGrounded;

    private Vector3 movementDirection;
    private float verticalVelocity;
    public float speed = 5f;
    public float jumpForce = 1.5f;
    public float gravity = -9.8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCharController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = playerCharController.isGrounded;

        MovePlayer();

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
}
