using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Weapon
{
    public class Gun : MonoBehaviour
    {
        private float damage;
        private float range;
        private Animator animator;
        public Camera fpsCam;
        public WEAPONSO weaponSO; // Reference to the weaponSO

        private float nextTimeToFire = 0f;
        private bool isShooting = false;

        void Start()
        {
            animator = GetComponent<Animator>();

            damage = weaponSO.Damage;
            range = weaponSO.Range;
            if (weaponSO.FireRate <= 0)
            {
                weaponSO.FireRate = 1f;
            }
            nextTimeToFire = Time.time;
        }

        void OnEnable()
        {
            isShooting = false; // Reset shooting state when weapon is activated
        }

        void OnDisable()
        {
            isShooting = false; // Ensure shooting stops when weapon is deactivated
        }

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
            // Ignore input if the weapon is not active
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (context.started)
            {
                if (!weaponSO.isAutomatic)
                {
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
            else if (context.canceled)
            {
                isShooting = false;
            }
        }

        void Shoot()
        {
            
            animator.Play("Shoot", 0, 0f);
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                
                
                    Destroy(hit.collider.gameObject);
                
            }
        }
    }
}