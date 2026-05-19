// PatrolModule.cs - Comportamiento de patrullaje
using UnityEngine;

public class PatrolModule : EnemyModule
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waypointReachDistance = 2f;
    [SerializeField] private float patrolSpeed = 0.5f;
    [SerializeField] private float waitTimeAtWaypoint = 0f;

    [Header("Patrol Type")]
    [SerializeField] private PatrolType patrolType = PatrolType.RandomWander;
    [SerializeField] private Transform[] patrolPoints;

    private Vector3 currentWaypoint;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    private MovementModule movementModule;

    public enum PatrolType
    {
        RandomWander,
        FixedPoints,
        CircularPath
    }

    protected override void OnInitialize()
    {
        movementModule = core.GetModule<MovementModule>();
        SetNewWaypoint();
    }

    public override void OnUpdate()
    {
        if (!isActive) return;

        // Solo patrullar si no hay objetivo
        if (sharedData.HasTarget) return;

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewWaypoint();
            }
            return;
        }

        // Moverse al waypoint
        if (movementModule != null)
        {
            movementModule.MoveToPosition(currentWaypoint, patrolSpeed);
        }

        // Verificar si llegamos
        float distance = Vector3.Distance(transform.position, currentWaypoint);
        if (distance < waypointReachDistance)
        {
            OnWaypointReached();
        }
    }

    private void OnWaypointReached()
    {
        if (waitTimeAtWaypoint > 0)
        {
            isWaiting = true;
            waitTimer = waitTimeAtWaypoint;
            if (movementModule != null)
            {
                movementModule.Stop();
            }
        }
        else
        {
            SetNewWaypoint();
        }
    }

    private void SetNewWaypoint()
    {
        switch (patrolType)
        {
            case PatrolType.RandomWander:
                SetRandomWaypoint();
                break;
            case PatrolType.FixedPoints:
                SetFixedPointWaypoint();
                break;
            case PatrolType.CircularPath:
                SetCircularWaypoint();
                break;
        }
    }

    private void SetRandomWaypoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        currentWaypoint = transform.position + new Vector3(
            randomCircle.x,
            Random.Range(-2f, 2f),
            randomCircle.y
        );
    }

    private void SetFixedPointWaypoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            SetRandomWaypoint();
            return;
        }

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        currentWaypoint = patrolPoints[currentPatrolIndex].position;
    }

    private void SetCircularWaypoint()
    {
        float angle = currentPatrolIndex * (360f / 8f) * Mathf.Deg2Rad; // 8 puntos en círculo
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * patrolRadius;
        currentWaypoint = transform.position + offset;
        currentPatrolIndex = (currentPatrolIndex + 1) % 8;
    }

    public override void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(currentWaypoint, 0.5f);
            Gizmos.DrawLine(transform.position, currentWaypoint);
        }

        if (patrolType == PatrolType.FixedPoints && patrolPoints != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.5f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
        }
    }
}