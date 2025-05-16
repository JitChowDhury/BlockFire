using UnityEngine;

public class DoorDetector : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void OnTriggerEnter(Collider other)
    {
        animator.SetTrigger("Open");
    }
    void OnTriggerExit(Collider other)
    {
        animator.SetTrigger("Close");
    }
}
