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
        }

        void OnEnable()
        {
            isFiring = false;
            isReloading = false;
        }

        void OnDisable()
        {
            isFiring = false;
            isReloading = false;
            StopAllCoroutines(); // Ensure reload coroutine stops
        }

        void Update()
        {
            if (isReloading)
                return;

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
            // Validate reload conditions
            if (currentMag <= 0 || isReloading || currentAmmo >= maxAmmo)
            {
                yield break;
            }

            // Start reload
            isReloading = true;
            animator.SetTrigger(Constants.RELOAD_ANIM);

            // Wait for animation duration
            yield return new WaitForSeconds(reloadClip.length);

            // Update ammo and mag
            if (currentMag > 0)
            {
                currentAmmo = maxAmmo;
                currentMag--;
                Debug.Log($"Reloaded. Ammo: {currentAmmo}/{maxAmmo}, Mags: {currentMag}/{maxMag}");
            }

            // Reset state
            isReloading = false;
        }

        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (context.started)
            {
                // Check if magazine is empty
                if (currentMag <= 0 && currentAmmo <= 0)
                {
                    if (emptyMagSound != null)
                        emptyMagSound.Play();
                    return;
                }

                // Handle non-automatic firing
                if (!isAutomatic && Time.time >= nextFireTime && currentAmmo > 0)
                {
                    nextFireTime = Time.time + 1f / fireRate;
                    Shoot();
                }
                // Start continuous firing for automatic weapons
                else if (isAutomatic)
                {
                    isFiring = true;
                }
            }
            else if (context.canceled)
            {
                isFiring = false;
            }
        }

        private void Shoot()
        {
            if (isReloading || currentAmmo <= 0)
                return;

            // Play shoot animation
            animator.SetTrigger(Constants.SHOOT_ANIM);

            // Perform raycast
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
                // Ignore player hits
                if (hit.collider.CompareTag(Constants.PLAYER_TAG))
                    return;

                // Spawn bullet impact
                if (bulletImpactPrefab != null)
                    Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));

                // Apply damage to enemies
                if (hit.collider.CompareTag(Constants.ENEMY_TAG))
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                        EventManager.RaiseOnEnemyDamage(enemy, damage);
                }
            }

            // Reduce ammo
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