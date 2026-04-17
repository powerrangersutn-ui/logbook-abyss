using UnityEngine;

public class EnemyMovementTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistance = 1.8f;
    [SerializeField] private float maxDistance = 8f;

    // Componentes
    private EnemyPatrol patrol;
    private EnemyAttack enemyAttack;        
    private EnemyHealth enemyHealth;
    private PlayerStats playerHealth;

    private bool isDead = false;

    void Start()
    {
        patrol = GetComponent<EnemyPatrol>();
        enemyAttack = GetComponent<EnemyAttack>();           
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth == null)
            enemyHealth = GetComponentInChildren<EnemyHealth>();

        if (player != null)
            playerHealth = player.GetComponent<PlayerStats>();
        else
            Debug.LogWarning("Player no asignado en " + gameObject.name, this);

        // Inicializar el sistema de ataque
        if (enemyAttack != null && player != null)
            enemyAttack.Initialize(player);
    }

    void Update()
    {
        if (IsEnemyDead())
            return;

        if (playerHealth != null && playerHealth.playerHealth <= 0)
        {
            patrol.playerDetected = false;
            return;
        }

        if (player == null) return;

        float currentDistance = Vector3.Distance(transform.position, player.position);

        patrol.playerDetected = currentDistance < maxDistance;

        if (patrol.playerDetected)
        {
            if (currentDistance > minDistance)
            {
                // Solo movimiento
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                transform.LookAt(player.position);
            }
            else
            {
                // Solo pedir que ataque (el cooldown y lógica están en EnemyAttack)
                enemyAttack?.TryAttack();
            }
        }
        else
        {
            // Fuera de rango
        }
    }

    private bool IsEnemyDead()
    {
        if (isDead) return true;

        if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0)
        {
            isDead = true;
            patrol.playerDetected = false;
            if (enemyAttack != null)
                enemyAttack.StopAttacking();
            return true;
        }
        return false;
    }
}