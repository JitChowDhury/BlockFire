using FPS.Core;
using FPS.Utility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform Player;
    private float distanceToPlayer;
    private float attackRange = 2f;
    private PlayerInput playerInput;

    [SerializeField] private float health = 40f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindWithTag(Constants.PLAYER_TAG).GetComponent<Transform>();
        playerInput = GameObject.Find("GameManager").GetComponent<PlayerInput>();

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
        agent.SetDestination(Player.position);
        CalculateDistanceToPlayer();

        if (distanceToPlayer <= attackRange)
        {
            GameObject.Find("GameManager").GetComponent<PlayerInput>().enabled = false;
        }
    }

    private void CalculateDistanceToPlayer()
    {
        if (Player == null) return;

        Vector3 enemyPosition = transform.position;
        Vector3 playerPosition = Player.transform.position;
        //same as (enemyPosition-playerPosition).magnitude
        distanceToPlayer = Vector3.Distance(enemyPosition, playerPosition);
    }

    void HandleDamage(float damage)
    {
        health -= damage;
    }

}
