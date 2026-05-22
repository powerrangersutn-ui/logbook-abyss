using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySensors sensors;

    [SerializeField] private AquaticLocomotion locomotion;

    [Header("Patrol")]
    [SerializeField] private PatrolPoint[] patrolPoints;

    [SerializeField] private float patrolReachDistance = 2f;

    [Header("Movement Speeds")]
    [SerializeField] private float patrolSpeed = 3f;

    [SerializeField] private float chaseSpeed = 9f;

    [SerializeField] private float screamSpeed = 0f;

    [Header("Scream")]
    [SerializeField] private float screamDuration = 2f;

    [Header("Memory")]
    [SerializeField] private float alertDuration = 5f;

    [Header("State")]
    [SerializeField] private EnemyState currentState;

    [Header("Advanced Chase")]
    [SerializeField] private float orbitDistance = 4f;

    [SerializeField] private float orbitHeightVariation = 2f;

    [SerializeField] private float orbitChangeInterval = 3f;

    private float orbitTimer;

    private Vector3 orbitOffset;

    public EnemyState CurrentState => currentState;

    private int patrolIndex;

    private float alertTimer;

    private float screamTimer;

    private bool screamTriggered;

    private Vector3 lastKnownTargetPosition;

    private void Start()
    {
        ChangeState(EnemyState.Patrol);
        GenerateOrbitOffset();
    }

    private void Update()
    {
        EvaluateState();
        ExecuteState();
    }
    private void GenerateOrbitOffset()
    {
        Vector3 side =
            Random.value > 0.5f
            ? sensors.Target.right
            : -sensors.Target.right;

        float height =
            Random.Range(
                -orbitHeightVariation,
                orbitHeightVariation);

        orbitOffset =
            side * orbitDistance +
            Vector3.up * height;

        orbitTimer = orbitChangeInterval;
    }
    private void EvaluateState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:

                if (sensors.CanSeeTarget)
                {
                    lastKnownTargetPosition =
                        sensors.Target.position;

                    ChangeState(EnemyState.Scream);
                }

                break;

            case EnemyState.Scream:

                screamTimer -= Time.deltaTime;

                if (screamTimer <= 0f)
                {
                    ChangeState(EnemyState.Chase);
                }

                break;

            case EnemyState.Chase:
                
                orbitTimer -= Time.deltaTime;


                if (orbitTimer <= 0f)
                {
                    GenerateOrbitOffset();
                }

                if (sensors.CanSeeTarget)
                {
                    lastKnownTargetPosition =
                        sensors.Target.position;

                    alertTimer = alertDuration;
                }
                else
                {
                    ChangeState(EnemyState.Alert);
                }

                break;

            case EnemyState.Alert:

                alertTimer -= Time.deltaTime;

                if (alertTimer <= 0f)
                {
                    ChangeState(EnemyState.Patrol);
                }

                break;
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:

                locomotion.SetMoveSpeed(patrolSpeed);

                HandlePatrol();

                break;

            case EnemyState.Scream:

                locomotion.SetMoveSpeed(screamSpeed);

                HandleScream();

                break;

            case EnemyState.Chase:

                locomotion.SetMoveSpeed(chaseSpeed);

                MoveTo(lastKnownTargetPosition);

                break;

            case EnemyState.Alert:

                locomotion.SetMoveSpeed(patrolSpeed);

                MoveTo(lastKnownTargetPosition);

                break;
        }
    }

    private void HandlePatrol()
    {
        if (patrolPoints.Length == 0)
        {
            locomotion.SetDesiredDirection(Vector3.zero);
            return;
        }

        PatrolPoint point =
            patrolPoints[patrolIndex];

        Vector3 targetPosition =
            point.transform.position;

        MoveTo(targetPosition);

        float distance =
            Vector3.Distance(
                transform.position,
                targetPosition);

        if (distance <= patrolReachDistance)
        {
            patrolIndex++;

            if (patrolIndex >= patrolPoints.Length)
                patrolIndex = 0;
        }
    }

    private void HandleScream()
    {
        if (sensors.Target == null)
            return;

        Vector3 lookDirection =
            sensors.Target.position - transform.position;

        locomotion.SetDesiredDirection(Vector3.zero);

        locomotion.SetForcedLookDirection(lookDirection);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction =
            targetPosition - transform.position;

        locomotion.SetDesiredDirection(direction);
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;

        locomotion.ClearForcedLookDirection();

        switch (newState)
        {
            case EnemyState.Scream:

                screamTimer = screamDuration;

                screamTriggered = false;

                TriggerScream();

                break;
        }
    }
    private void HandleAdvancedChase()
    {
        if (sensors.Target == null)
            return;

        Vector3 targetPosition =
            sensors.Target.position + orbitOffset;

        MoveTo(targetPosition);
    }

    private void TriggerScream()
    {
        if (screamTriggered)
            return;

        screamTriggered = true;

        Debug.Log("SIREN SCREAM");
    }
}