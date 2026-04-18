using UnityEngine;

public class EnemyMovementTowardsPlayer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;

    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistance = 1.8f;
    [SerializeField] private float maxDistance = 8f;

    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 2f;

    private EnemyWeapon enemyWeapon;
    private float lastAttackTime = -999f;

    void Start()
    {
        // Buscar el EnemyWeapon en este objeto o en sus hijos
        enemyWeapon = GetComponentInChildren<EnemyWeapon>();

        if (enemyWeapon == null)
        {
            Debug.LogError($"NO SE ENCONTRO EnemyWeapon en {gameObject.name} ni en sus hijos");
        }
        else
        {
            Debug.Log($"EnemyWeapon encontrado en: {enemyWeapon.gameObject.name}");
        }

        if (player == null)
        {
            Debug.LogError($"PLAYER NO ASIGNADO en {gameObject.name}");
        }
    }

    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);

        // Si esta cerca
        if (distance < maxDistance)
        {
            GetComponent<EnemyPatrol>().playerDetected = true;
            // Si esta muy cerca, atacar
            if (distance <= minDistance)
            {
                // Mirar al player
                
                transform.LookAt(player.position);

                // Intentar atacar si paso el cooldown
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // Moverse hacia el player
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                transform.LookAt(player.position);
            }
            return;
        }
        GetComponent<EnemyPatrol>().playerDetected = false;
    }

    private void Attack()
    {
        Debug.Log($">>> ATACANDO desde {gameObject.name}");

        if (enemyWeapon != null)
        {
            lastAttackTime = Time.time;
            enemyWeapon.StartAttack();
        }
        else
        {
            Debug.LogError("No se puede atacar: enemyWeapon es null");
        }
    }
}