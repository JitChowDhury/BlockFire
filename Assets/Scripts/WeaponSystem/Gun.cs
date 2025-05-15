using UnityEngine;
using UnityEngine.InputSystem;
using FPS.Utility;
using Unity.Mathematics;
using System.Collections;
using FPS.Core;
using UnityEngine.UI;

namespace FPS.Weapon
{
    public class Gun : MonoBehaviour
    {
        private float damage;
        private float range;
        private Animator animator;
        private int currentAmmo;
        private bool isReloading = false;

        public Camera fpsCam;
        public WEAPONSO weaponSO; // Reference to the weaponSO
        [SerializeField] private GameObject bulletImpact;
        [SerializeField] private AnimationClip reload;
        [SerializeField] private Image crossHair;

        private float nextTimeToFire = 0f;
        private bool isShooting = false;

        void Start()
        {
            currentAmmo = weaponSO.maxAmmo;
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
            HandleCrossHair();
            if (isReloading) return;
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }


            if (isShooting && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / weaponSO.FireRate;
                Shoot();
            }
        }

        IEnumerator Reload()
        {
            isReloading = true;
            animator.Play(Constants.RELOAD_ANIM, 0, 0f);

            yield return new WaitForSeconds(reload.length);
            currentAmmo = weaponSO.maxAmmo;
            isReloading = false;



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


        void HandleCrossHair()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                if (hit.collider.CompareTag(Constants.ENEMY_TAG))
                {

                    crossHair.color = Color.red;
                }
                else
                {
                    crossHair.color = Color.white;
                }
            }
        }

        void Shoot()
        {
            if (isReloading) return;
            if (currentAmmo <= 0) return;

            animator.Play(Constants.SHOOT_ANIM, 0, 0f);
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                if (hit.collider.CompareTag(Constants.PLAYER_TAG)) return;
                Instantiate(bulletImpact, hit.point, quaternion.identity);
                currentAmmo--;
                if (hit.collider.CompareTag(Constants.ENEMY_TAG))
                    EventManager.RaiseOnEnemyDamage(hit.collider.GetComponent<Enemy>(), weaponSO.Damage);


            }
        }
    }
}