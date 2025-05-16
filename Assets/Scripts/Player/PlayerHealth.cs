using FPS.Core;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    public float health;
    private PlayerInput playerInput;
    void Start()
    {
        playerInput = GameObject.Find("GameManager").GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        EventManager.OnPlayerDamage += HandlePlayerDamage;
    }

    void OnDisable()
    {
        EventManager.OnPlayerDamage -= HandlePlayerDamage;

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Debug.Log("Player is Dead");
            playerInput.enabled = false;
        }
    }

    public void HandlePlayerDamage(float damgeAmount)
    {
        health -= damgeAmount;
    }
}
