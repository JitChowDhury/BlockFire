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
        // Weapon properties
        private float damage;
        private float range;
        private float fireRate;
        private bool isAutomatic;

        // Ammo and magazine
        private int currentAmmo;
        private int maxAmmo;
        private int currentMag;
        private int maxMag;

        // State
        public bool isReloading { get; private set; } = false;
        private bool isFiring = false;
        private float nextFireTime = 0f;

        // Components and references
        private Animator animator;
        public Camera fpsCam;
        public WEAPONSO weaponSO;
        [SerializeField] private GameObject bulletImpactPrefab;
        [SerializeField] private AnimationClip reloadClip;
        [SerializeField] private Image crosshairImage;
        [SerializeField] private AudioSource emptyMagSound;

        void Start()
        {
            // Initialize weapon properties from ScriptableObject
            damage = weaponSO.Damage;
            range = weaponSO.Range;
            fireRate = weaponSO.FireRate > 0 ? weaponSO.FireRate : 1f;
            isAutomatic = weaponSO.isAutomatic;
            maxAmmo = weaponSO.maxAmmo;
            maxMag = weaponSO.maxMag;

            // Initialize ammo and magazines
            currentAmmo = maxAmmo;
            currentMag = maxMag;

            // Get components
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on Gun GameObject!");
            }
        }

        void OnEnable()
        {
            isFiring = false;
            isReloading = false;
            Debug.Log("Gun enabled, isFiring reset to false");
        }

        void OnDisable()
        {
            isFiring = false;
            isReloading = false;
            StopAllCoroutines();
            Debug.Log("Gun disabled, isFiring and isReloading reset");
        }

        void Update()
        {
            if (isReloading)
            {
                Debug.Log($"Reloading, Update blocked. Animator state: {(animator.GetCurrentAnimatorStateInfo(0).IsName("Reload") ? "Reload" : "Other")}");
                return;
            }

            // Handle crosshair color
            UpdateCrosshair();

            // Check for reload condition
            if (currentAmmo <= 0 && currentMag > 0 && !isReloading)
            {
                StartCoroutine(Reload());
                return;
            }

            // Handle continuous firing for automatic weapons
            if (isFiring && isAutomatic && Time.time >= nextFireTime && currentAmmo > 0)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }
        }

        IEnumerator Reload()
        {
            if (currentMag <= 0 || isReloading || currentAmmo >= maxAmmo)
            {
                Debug.Log("Cannot reload: No mags, already reloading, or ammo full");
                yield break;
            }

            isReloading = true;
            Debug.Log($"Starting reload animation, Trigger: {Constants.RELOAD_ANIM}");
            animator.SetTrigger(Constants.RELOAD_ANIM);

            // Wait until Reload state is active
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Reload"));
            float stateLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log($"Reload state active, duration: {stateLength}s (Clip length: {reloadClip.length}s)");
            yield return new WaitForSeconds(stateLength + 0.5f);

            if (currentMag > 0)
            {
                currentAmmo = maxAmmo;
                currentMag--;
                Debug.Log($"Reloaded. Ammo: {currentAmmo}/{maxAmmo}, Mags: {currentMag}/{maxMag}");
            }

            isReloading = false;
            isFiring = false;
            Debug.Log("Reload complete, isFiring reset to false");
        }

        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (!gameObject.activeInHierarchy || isReloading)
            {
                Debug.Log("Shoot input ignored: Inactive or reloading");
                return;
            }

            if (context.started)
            {
                Debug.Log("Shoot input started");
                if (currentMag <= 0 && currentAmmo <= 0)
                {
                    emptyMagSound?.Play();
                    Debug.Log("Empty mag, cannot shoot");
                    return;
                }

                if (!isAutomatic && Time.time >= nextFireTime && currentAmmo > 0)
                {
                    nextFireTime = Time.time + 1f / fireRate;
                    Shoot();
                }
                else if (isAutomatic && currentAmmo > 0)
                {
                    isFiring = true;
                    Debug.Log("isFiring set to true");
                }
            }
            else if (context.canceled)
            {
                isFiring = false;
                Debug.Log("Shoot input canceled, isFiring set to false");
            }
        }

        private void Shoot()
        {
            if (isReloading || currentAmmo <= 0)
            {
                Debug.Log("Cannot shoot: Reloading or no ammo");
                return;
            }

            Debug.Log("Shoot triggered");
            animator.SetTrigger(Constants.SHOOT_ANIM);

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                if (hit.collider.CompareTag(Constants.PLAYER_TAG))
                    return;

                if (bulletImpactPrefab != null)
                    Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));

                if (hit.collider.CompareTag(Constants.ENEMY_TAG))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                        EventManager.RaiseOnEnemyDamage(enemy, damage);
                }
            }

            currentAmmo--;
        }

        private void UpdateCrosshair()
        {
            if (crosshairImage == null)
                return;

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                crosshairImage.color = hit.collider.CompareTag(Constants.ENEMY_TAG) ? Color.red : Color.white;
            }
            else
            {
                crosshairImage.color = Color.white;
            }
        }

        public int GetCurrentAmmo()
        {
            return currentAmmo;
        }

        public int GetCurrentMag()
        {
            return currentMag;
        }
    }
}