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
        private int magCap;
        private int currentMag;
        private bool isReloading = false;

        public Camera fpsCam;
        public WEAPONSO weaponSO; // Reference to the weaponSO
        [SerializeField] private GameObject bulletImpact;
        [SerializeField] private AnimationClip reload;
        [SerializeField] private Image crossHair;
        [SerializeField] private AudioSource emptymag;

        private float nextTimeToFire = 0f;
        private bool isShooting = false;

        void Start()
        {
            magCap = weaponSO.maxMag;
            currentMag = magCap;
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
            if (isReloading) return;

            // Stop all actions if magazine is completely empty
            if (currentMag == 0)
            {
                return;
            }

            HandleCrossHair();

            // Only attempt to reload if we have magazines and ammo is depleted
            if (currentAmmo <= 0 && currentMag > 0)
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
            if (currentMag <= 0)
            {
                yield break;
            }
            isReloading = true;
            animator.Play(Constants.RELOAD_ANIM, 0, 0f);

            yield return new WaitForSeconds(reload.length);
            currentAmmo = weaponSO.maxAmmo;
            currentMag--;
            Debug.Log("Mag cap is now " + currentMag);
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
                // If magazine is empty, play click sound and do nothing else
                if (currentMag == 0)
                {
                    if (emptymag != null)
                    {
                        emptymag.Play();
                    }
                    return;
                }

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