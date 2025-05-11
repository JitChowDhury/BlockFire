using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    // [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 1.1f, 0f); // Adjust for slow rotation
    public float rotationSpeed = 2.0f; // Degrees per second
    void Update()
    {
        // Rotate the directional light based on rotationSpeed
        transform.Rotate(0,rotationSpeed * Time.deltaTime,0, Space.World);
    }
}
