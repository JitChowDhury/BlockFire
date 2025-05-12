using UnityEngine;
using UnityEngine.InputSystem;

// Namespace to organize player-related scripts
namespace FPS.Player
{
    public class Movement : MonoBehaviour
    {
        // Reference to the CharacterController for movement and collision handling
        private CharacterController playerCharController;
        // Tracks if the character is touching the ground, used for jumping
        private bool isGrounded;

        // Local movement direction (X, Z) from WASD input, transformed to world space later
        private Vector3 movementDirection;
        // Look input (X for yaw, Y for pitch) from mouse or gamepad
        private Vector2 lookInput;
        // Vertical velocity (Y-axis) for jumping and falling
        private float verticalVelocity;

        public float speed = 5f;
        // Jump force, determines jump height
        public float jumpForce = 1.5f;
        // Gravity acceleration, negative to pull downward
        public float gravity = -9.8f;

        // Sensitivity for look rotation speed (mouse/gamepad), adjustable in Inspector
        public float lookSensitivity = 100f;
        // Tracks camera's vertical (pitch) angle for up/down looking
        private float pitch = 0f;


        public Transform cameraTransform;

        // Called once at start to initialize components
        void Start()
        {

            playerCharController = GetComponent<CharacterController>();

            if (cameraTransform != null)
            {

                Cursor.lockState = CursorLockMode.Locked;

                Cursor.visible = false;
            }
        }

        // Called every frame to update movement and rotation
        void Update()
        {
            if (Cursor.visible) return;//player will not move if cursor is not on game
            // Update grounded state for jump logic
            isGrounded = playerCharController.isGrounded;

            // Handle movement and jumping
            MovePlayer();
            // Handle look rotation (yaw and pitch)
            Applylook();
        }


        void Applylook()
        {
            // Calculate yaw (horizontal rotation) from X input, scaled by sensitivity and time
            float yaw = lookInput.x * lookSensitivity * Time.deltaTime;
            // Rotate character around Y-axis for left/right turning
            // Effect: Changes character's facing direction, affecting movement
            transform.Rotate(0, yaw, 0);

            // If camera is assigned, handle pitch (vertical rotation)
            if (cameraTransform != null)
            {
                // Calculate pitch from Y input, subtract for natural mouse feel (down = look up)
                pitch -= lookInput.y * lookSensitivity * Time.deltaTime;
                // Clamp pitch to prevent camera flipping 
                pitch = Mathf.Clamp(pitch, -80f, 80f);
                // Apply pitch to camera's local X rotation, keeping character level
                // Effect: Camera tilts up/down for first-person view
                cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
            }
        }

        // Handles movement, jumping, and gravity for the CharacterController
        void MovePlayer()
        {
            // Apply gravity to vertical velocity each frame to simulate falling
            // Note: Negative gravity value pulls downward
            verticalVelocity += gravity * Time.deltaTime;

            // When grounded and falling, reset vertical velocity to keep controller grounded
            // Effect: Prevents floating/gaps by applying a small downward force
            if (playerCharController.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -0.1f;
            }

            // Convert local movement direction to world space based on character's rotation
            // Effect: Ensures WASD movement aligns with where character is facing
            Vector3 worldMovementDirection = transform.TransformDirection(movementDirection);
            // Add vertical velocity (jump/fall) to the movement vector
            worldMovementDirection.y = verticalVelocity;
            // Move the CharacterController with the combined motion, scaled by speed and time
            // Effect: Moves character in world space, handling collisions
            playerCharController.Move(worldMovementDirection * speed * Time.deltaTime);
            // Debug log to monitor movement direction for testing
            Debug.Log(movementDirection);
        }

        // Handles movement input from WASD or gamepad left stick
        public void HandleMove(InputAction.CallbackContext context)
        {
            // Read Vector2 input (X for left/right, Y for forward/back)
            Vector3 input = context.ReadValue<Vector2>();
            // Convert to local movement direction (X, Z), Y is 0 (vertical handled separately)
            // Effect: Sets direction for MovePlayer to process
            movementDirection = new Vector3(input.x, 0, input.y);
        }

        // Handles jump input from Space key
        public void HandleJump(InputAction.CallbackContext context)
        {
            // Only jump if Space is pressed and character is grounded
            // Effect: Prevents mid-air jumps and ensures single jump per press
            if (!context.performed || !isGrounded) return;
            // Apply jump force to vertical velocity for upward motion
            // Effect: Initiates jump, countered by gravity later
            verticalVelocity = jumpForce;
        }

        // Handles look input from mouse or gamepad right stick
        public void HandleLook(InputAction.CallbackContext context)
        {
            // Read Vector2 input (X for yaw, Y for pitch)
            // Effect: Updates look direction for Applylook to process
            lookInput = context.ReadValue<Vector2>();
        }
    }
}