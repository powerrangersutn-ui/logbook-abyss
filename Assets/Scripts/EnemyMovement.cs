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

//Adjust the detection distance and speed for the enemy here
    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistance = 1.8f;
    [SerializeField] private float maxDistance = 8f;

    private EnemyPatrol patrol;

    void Start()
    {
        
        patrol = GetComponent<EnemyPatrol>();
    }

    void Update()
    {
        //Current distance between the enemy and the player
        float currentDistance = Vector3.Distance(transform.position, player.position);

        //If the player is within range of maxDistance, the enemy detect them
        patrol.playerDetected = currentDistance < maxDistance;

        if (patrol.playerDetected) 
        {
            if (currentDistance > minDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                //Look at the player
                transform.LookAt(player.position);
            }
                //If the enemy is close enough to the player, stop and attack
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
            // no se bien que hacia esto, pero creo que resetea al timer del ataque para que ataque de nuevo (nota: corregir si me equivoco porfa)
            lastAttackTime = Time.time; 
            Debug.Log("Atacando");
        }
    }

    

}
