using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private CharacterController playerCharController;

    Vector3 movementDirection;
    public float speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCharController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }


    void MovePlayer()
    {
        playerCharController.Move(transform.TransformDirection(movementDirection) * speed * Time.deltaTime);
        Debug.Log(movementDirection);
    }
    public void HandleMove(InputAction.CallbackContext context)
    {


        Vector3 input = context.ReadValue<Vector2>();
        movementDirection = new Vector3(input.x, 0, input.y);

    }
}
