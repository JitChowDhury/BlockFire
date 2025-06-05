using System;
using UnityEngine;

namespace FPS.Enemies
{
    public class Drone : MonoBehaviour
    {
        [SerializeField] private float chaseRange; // Range within which drone starts chasing player
        [SerializeField] private float attackRange; // Range within which drone stops to attack
        [SerializeField] private Transform player; // Reference to player transform
        [SerializeField] private float speed; // Movement speed of drone
        [SerializeField] private GameObject Light; // Light that turns on when chasing

        void Start()
        {
            Light.SetActive(false); // Disable light initially
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, player.position); // Distance to player

            if (distance < chaseRange)
            {
                Light.SetActive(true); // Enable light when in chase range

                // Move toward player
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

                // Rotate smoothly toward player
                Vector3 direction = player.position - transform.position;
                direction.y = 0; // Prevent tilting up/down

                Quaternion startRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, Time.deltaTime);
            }

            if (distance < attackRange)
            {
                speed = 0; // Stop moving when in attack range (placeholder for damage logic)
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange); // Draw chase range in editor
        }
    }
}