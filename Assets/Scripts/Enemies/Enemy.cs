using FPS.Utility;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindWithTag(Constants.PLAYER_TAG).GetComponent<Transform>();

    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(Player.position);
    }
}
