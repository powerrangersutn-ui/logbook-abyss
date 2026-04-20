using UnityEngine;

public class EnemyMovementTowardsPlayer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player;

    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistance = 1.8f;
    [SerializeField] private float maxDistance = 8f;

    [Header("Huir con Vitácora")]
    [SerializeField] private float fleeDistance = 10f;        // Distancia a la que empiezan a huir
    [SerializeField] private float fleeSpeedMultiplier = 2f;  // 2 = doble de velocidad
    [SerializeField] private float fleeDirection = 180f;      // Ángulo de huida (180 = opuesto al jugador, 0-360)
    [SerializeField] private float maxFleeDistance = 20f;     // Distancia máxima de huida antes de destruirse

    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 2f;

    private EnemyWeapon enemyWeapon;
    private float lastAttackTime = -999f;
    private bool isFleeing = false;
    private Vector3 fleeStartPosition;

    private void Start()
    {
        enemyWeapon = GetComponentInChildren<EnemyWeapon>();
        if (enemyWeapon == null)
            Debug.LogError($"NO SE ENCONTRO EnemyWeapon en {gameObject.name}");
        if (player == null)
            Debug.LogError($"PLAYER NO ASIGNADO en {gameObject.name}");
    }

    private void Update()
    {
        if (player == null) return;

        // Si ya está huyendo, solo verificar distancia recorrida
        if (isFleeing)
        {
            ContinueFleeing();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        // ====================== NUEVA LÓGICA: HUIR CON VITÁCORA ======================
        if (GameManager.Instance.hasLogbook && distance < fleeDistance)
        {
            StartFleeing();
            return;
        }

        // ====================== LÓGICA NORMAL (sin vitácora o fuera de rango) ======================
        if (distance < maxDistance)
        {
            GetComponent<EnemyPatrol>().playerDetected = true;
            if (distance <= minDistance)
            {
                // Atacar
                transform.LookAt(player.position);
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                // Perseguir
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                transform.LookAt(player.position);
            }
            return;
        }

        GetComponent<EnemyPatrol>().playerDetected = false;
    }

    // ====================== NUEVO MÉTODO: INICIAR HUIDA ======================
    private void StartFleeing()
    {
        isFleeing = true;
        fleeStartPosition = transform.position;
        Debug.Log($"{gameObject.name} está huyendo con la vitácora!");
    }

    // ====================== NUEVO MÉTODO: CONTINUAR HUIDA ======================
    private void ContinueFleeing()
    {
        // Calcular dirección de huida basada en el ángulo configurado
        Vector3 directionAway;

        if (fleeDirection == 180f)
        {
            // Huir en dirección opuesta al jugador (comportamiento original)
            directionAway = (transform.position - player.position).normalized;
        }
        else
        {
            // Huir en una dirección específica (en grados)
            float radians = fleeDirection * Mathf.Deg2Rad;
            directionAway = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)).normalized;
        }

        // Velocidad de huida
        float currentFleeSpeed = speed * fleeSpeedMultiplier;

        // Moverse en dirección de huida
        transform.position += directionAway * currentFleeSpeed * Time.deltaTime;

        // Mirar hacia donde está huyendo
        transform.LookAt(transform.position + directionAway);

        // Verificar si alcanzó la distancia máxima de huida
        float distanceFled = Vector3.Distance(fleeStartPosition, transform.position);

        if (distanceFled >= maxFleeDistance)
        {
            Debug.Log($"{gameObject.name} se destruye después de huir {distanceFled:F2} metros");
            Destroy(gameObject);
        }
    }

    private void Attack()
    {
        Debug.Log($">>> ATACANDO desde {gameObject.name}");
        if (enemyWeapon != null)
        {
            lastAttackTime = Time.time;
            enemyWeapon.StartAttack();
        }
    }
}