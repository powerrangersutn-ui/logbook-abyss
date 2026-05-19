// RetreatModule.cs - Comportamiento de retirada
using UnityEngine;

public class RetreatModule : EnemyModule
{
    [Header("Retreat Conditions")]
    [SerializeField] private float retreatHealthThreshold = 0.3f; // 30% salud
    [SerializeField] private bool retreatWhenOutnumbered = true;
    [SerializeField] private int minimumAlliesRequired = 1;

    [Header("Retreat Behavior")]
    [SerializeField] private float retreatDistance = 10f;
    [SerializeField] private float retreatSpeed = 1.3f;
    [SerializeField] private bool seekCover = false;
    [SerializeField] private LayerMask coverLayer;

    [Header("Recovery")]
    [SerializeField] private bool canRecover = true;
    [SerializeField] private float recoverHealthThreshold = 0.6f;
    [SerializeField] private float recoveryTime = 3f;

    private bool isRetreating = false;
    private Vector3 retreatPosition;
    private float recoveryTimer = 0f;

    private MovementModule movementModule;
    private GroupBehaviorModule groupModule;

    protected override void OnInitialize()
    {
        movementModule = core.GetModule<MovementModule>();
        groupModule = core.GetModule<GroupBehaviorModule>();
    }

    public override void OnUpdate()
    {
        if (!isActive || movementModule == null) return;

        // Verificar condiciones de retirada
        bool shouldRetreat = CheckRetreatConditions();

        if (shouldRetreat && !isRetreating)
        {
            StartRetreat();
        }
        else if (!shouldRetreat && isRetreating)
        {
            if (canRecover)
            {
                recoveryTimer += Time.deltaTime;
                if (recoveryTimer >= recoveryTime)
                {
                    StopRetreat();
                }
            }
        }

        // Ejecutar retirada
        if (isRetreating)
        {
            ExecuteRetreat();
        }
    }

    private bool CheckRetreatConditions()
    {
        // Condición de salud
        if (sharedData.HealthPercent <= retreatHealthThreshold)
        {
            return true;
        }

        // Condición de grupo
        if (retreatWhenOutnumbered && groupModule != null)
        {
            if (sharedData.nearbyAlliesCount < minimumAlliesRequired)
            {
                return true;
            }
        }

        return false;
    }

    private void StartRetreat()
    {
        isRetreating = true;
        sharedData.isRetreating = true;
        recoveryTimer = 0f;

        // Calcular posición de retirada
        if (sharedData.HasTarget)
        {
            Vector3 fleeDirection = (transform.position - sharedData.currentTarget.position).normalized;

            if (seekCover)
            {
                retreatPosition = FindCoverPosition(fleeDirection);
            }
            else
            {
                retreatPosition = transform.position + fleeDirection * retreatDistance;
            }
        }
        else
        {
            // Retirarse en dirección aleatoria
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            retreatPosition = transform.position + new Vector3(randomDir.x, 0, randomDir.y) * retreatDistance;
        }
    }

    private void StopRetreat()
    {
        isRetreating = false;
        sharedData.isRetreating = false;
        recoveryTimer = 0f;
    }

    private void ExecuteRetreat()
    {
        movementModule.MoveToPosition(retreatPosition, retreatSpeed);

        // Si llegamos, buscar nueva posición
        float distance = Vector3.Distance(transform.position, retreatPosition);
        if (distance < 2f && sharedData.HasTarget)
        {
            Vector3 fleeDirection = (transform.position - sharedData.currentTarget.position).normalized;
            retreatPosition = transform.position + fleeDirection * retreatDistance;
        }
    }

    private Vector3 FindCoverPosition(Vector3 fleeDirection)
    {
        // Buscar cobertura en la dirección de huida
        RaycastHit hit;
        Vector3 searchPosition = transform.position + fleeDirection * retreatDistance;

        if (Physics.Raycast(searchPosition + Vector3.up * 5f, Vector3.down, out hit, 10f, coverLayer))
        {
            return hit.point;
        }

        return searchPosition;
    }

    public bool IsRetreating => isRetreating;

    public override void DrawGizmos()
    {
        if (!Application.isPlaying || !isRetreating) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, retreatPosition);
        Gizmos.DrawWireSphere(retreatPosition, 1f);
    }
}