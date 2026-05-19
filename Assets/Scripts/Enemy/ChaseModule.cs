// ChaseModule.cs - Persecución de objetivos
using UnityEngine;

public class ChaseModule : EnemyModule
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 1.2f;
    [SerializeField] private float minimumChaseDistance = 1f;
    [SerializeField] private bool predictTargetMovement = false;
    [SerializeField] private float predictionTime = 0.5f;

    [Header("Chase Behavior")]
    [SerializeField] private ChaseType chaseType = ChaseType.Direct;
    [SerializeField] private float circleRadius = 5f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float zigzagFrequency = 1f;

    private MovementModule movementModule;
    private Vector3 lastTargetPosition;
    private float zigzagTime = 0f;

    public enum ChaseType
    {
        Direct,          // Persecución directa
        Circle,          // Circular alrededor del objetivo
        Zigzag,          // Zigzag hacia el objetivo
        Intercept        // Interceptar trayectoria
    }

    protected override void OnInitialize()
    {
        movementModule = core.GetModule<MovementModule>();
    }

    public override void OnUpdate()
    {
        if (!isActive || movementModule == null) return;
        if (!sharedData.HasTarget) return;

        // Verificar si el módulo de ataque está en rango
        var attackModule = core.GetModule<MeleeAttackModule>();
        if (attackModule != null && attackModule.IsInAttackRange())
        {
            // El módulo de ataque se encarga del movimiento
            return;
        }

        Vector3 targetPosition = CalculateChasePosition();

        // Verificar distancia mínima
        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance > minimumChaseDistance)
        {
            movementModule.MoveToPosition(targetPosition, chaseSpeed);
        }
        else
        {
            movementModule.Stop();
        }

        lastTargetPosition = sharedData.currentTarget.position;
    }

    private Vector3 CalculateChasePosition()
    {
        Vector3 targetPos = sharedData.currentTarget.position;

        // Predicción de movimiento
        if (predictTargetMovement)
        {
            Vector3 targetVelocity = (targetPos - lastTargetPosition) / Time.deltaTime;
            targetPos += targetVelocity * predictionTime;
        }

        switch (chaseType)
        {
            case ChaseType.Direct:
                return targetPos;

            case ChaseType.Circle:
                return CalculateCirclePosition(targetPos);

            case ChaseType.Zigzag:
                return CalculateZigzagPosition(targetPos);

            case ChaseType.Intercept:
                return CalculateInterceptPosition(targetPos);

            default:
                return targetPos;
        }
    }

    private Vector3 CalculateCirclePosition(Vector3 targetPos)
    {
        // Orbitar alrededor del objetivo
        Vector3 toEnemy = transform.position - targetPos;
        Vector3 perpendicular = Vector3.Cross(toEnemy.normalized, Vector3.up);
        return targetPos + perpendicular * circleRadius;
    }

    private Vector3 CalculateZigzagPosition(Vector3 targetPos)
    {
        zigzagTime += Time.deltaTime * zigzagFrequency;

        Vector3 directionToTarget = (targetPos - transform.position).normalized;
        Vector3 perpendicular = Vector3.Cross(directionToTarget, Vector3.up);
        float offset = Mathf.Sin(zigzagTime) * zigzagAmplitude;

        return targetPos + perpendicular * offset;
    }

    private Vector3 CalculateInterceptPosition(Vector3 targetPos)
    {
        // Calcular punto de intercepción
        Vector3 targetVelocity = (targetPos - lastTargetPosition) / Time.deltaTime;
        float timeToIntercept = Vector3.Distance(transform.position, targetPos) / (stats.moveSpeed * chaseSpeed);

        return targetPos + targetVelocity * timeToIntercept;
    }

    public override void DrawGizmos()
    {
        if (!Application.isPlaying || !sharedData.HasTarget) return;

        Gizmos.color = Color.magenta;
        Vector3 chasePos = CalculateChasePosition();
        Gizmos.DrawLine(transform.position, chasePos);
        Gizmos.DrawWireSphere(chasePos, 0.5f);
    }
}