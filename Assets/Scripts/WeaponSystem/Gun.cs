using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    public WEAPONSO weaponSO;


    private float nextTimeToFire = 0f;
    private bool isShooting = false;
    // Update is called once per frame
    void Update()
    {
        if (isShooting && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / weaponSO.FireRate;
            Shoot();
        }
    }

    public void HandleShoot(InputAction.CallbackContext context)
    {
        // When button is pressed
        if (context.started)
        {
            if (!weaponSO.isAutomatic)
            {
                // Only shoot if cooldown has passed
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / weaponSO.FireRate;
                    Shoot();
                }
            }
            else
            {
                isShooting = true;
            }
        }

        // When button is released
        else if (context.canceled)
        {
            isShooting = false;
        }
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
