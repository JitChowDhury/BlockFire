using System;
using UnityEngine;

namespace  FPS.Enemies
{
    public class Drone : MonoBehaviour
    {
        [SerializeField] private float chaseRange;

        [SerializeField] private float attackRange;
        [SerializeField] private Transform player;
        [SerializeField] private float speed;

        [SerializeField] private GameObject Light;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Light.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < chaseRange)
        {
            Light.SetActive(true);
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed*Time.deltaTime);

            Vector3 direction = player.position - transform.position;
            direction.y = 0;
            
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, Time.deltaTime);
        }

        if (distance < attackRange)
        {
            //damage logic
            speed = 0;
        }
        
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,chaseRange);
            
        }
    }
}

