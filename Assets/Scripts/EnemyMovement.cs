using System.Collections;
//using UnityEditor.Rendering;
using UnityEngine;
//using UnityEngine.InputSystem.LowLevel;
//using UnityEngine.Timeline;

public class EnemyMovementTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float attackCooldown = 2f;
    public bool attacking;
    private float lastAttackTime;

// Adjust the detection distance and speed for the enemy here
    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistance = 1.8f;
    [SerializeField] private float maxDistance = 8f;

    private EnemyPatrol patrol;
    private EnemyWeapon enemyWeapon;
    private PlayerStats playerHealth;
    void Start()
    {
        
        patrol = GetComponent<EnemyPatrol>();
        enemyWeapon = GetComponentInChildren<EnemyWeapon>(true);
        playerHealth = player.GetComponent<PlayerStats>();
        
    }

    void Update()
    {
        if (playerHealth.playerHealth <= 0)
        {
            patrol.playerDetected = false;
            attacking = false;
            return;
        }
        // Current distance between the enemy and the player
        float currentDistance = Vector3.Distance(transform.position, player.position);

        // If the player is within range of maxDistance, the enemy detect them
        patrol.playerDetected = currentDistance < maxDistance;

        if (patrol.playerDetected) 
        {
            if (currentDistance > minDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                // Look at the player
                transform.LookAt(player.position);
            }
                // If the enemy is close enough to the player, stop and attack
                else
                {
                    Attack();
                }
        }
    }

           
    private void Attack()
    {   
        // If enough time has passed since the last attack, attack again
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Debug.Log("Attack() llamado");
            // No se bien que hacia esto, pero creo que toma el timer del juego haciendo que el ataque espere para que ataque de nuevo (nota: corregir si me equivoco porfavor)
            lastAttackTime = Time.time;
            attacking = true;
            Debug.Log("Ejecutando ataque onhitbox");
            // Como no se agregan animaciones por ahora, esto desactiva el collider del ataque.
            StartCoroutine(enemyWeapon.ActivateAttack());
        }
        else
        {
            attacking = false;
        }
    }

    

}
