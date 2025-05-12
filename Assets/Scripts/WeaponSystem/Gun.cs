using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    // Update is called once per frame
    void Update()
    {

    }

    public void HandleShoot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Shoot();
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Destroy(hit.collider.gameObject);
        }

    }
}
