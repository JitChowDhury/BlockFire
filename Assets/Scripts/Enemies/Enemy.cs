using FPS.Core;
using FPS.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace FPS.Enemies
{


    public class Enemy : MonoBehaviour
    {
        [SerializeField] GameObject explosionPrefab;
        [SerializeField] private float health = 40f;
        [SerializeField] private float damageAmount = 25f;

        private NavMeshAgent agent;
        private Transform player;
        private float distanceToPlayer;
        private float attackRange = 2f;

        private bool hasExploded;



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            player = GameObject.FindWithTag(Constants.PLAYER_TAG).GetComponent<Transform>();


        }

        void OnEnable()
        {
            EventManager.OnEnemyDamage += HandleDamage;
        }

        void OnDisable()
        {
            EventManager.OnEnemyDamage -= HandleDamage;
        }

        void Update()
        {
            if (health <= 0) Destroy(gameObject);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (hasExploded) return;

            agent.SetDestination(player.position);
            CalculateDistanceToPlayer();

            if (distanceToPlayer <= attackRange)
            {
                hasExploded = true;
                Instantiate(explosionPrefab, transform.position, quaternion.identity);
                Destroy(gameObject);
                EventManager.RaiseOnPlayerDamage(damageAmount);

            }
        }

        private void CalculateDistanceToPlayer()
        {
            if (player == null) return;

            Vector3 enemyPosition = transform.position;
            Vector3 playerPosition = player.transform.position;
            //same as (enemyPosition-playerPosition).magnitude
            distanceToPlayer = Vector3.Distance(enemyPosition, playerPosition);
        }

        void HandleDamage(Enemy target, float damage)
        {
            if (target != this) return;
            health -= damage;

        }

    }
}
