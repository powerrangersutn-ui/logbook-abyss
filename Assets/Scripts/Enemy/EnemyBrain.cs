using System;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySensors sensors;
    [SerializeField] private AquaticLocomotion locomotion;
    [SerializeField] private EnemyAttack attack;
    [SerializeField] private EnemyAnimator enemyAnimator;

    [Header("Patrol")]
    [SerializeField] private PatrolPoint[] patrolPoints;
    [SerializeField] private float patrolReachDistance = 2f;

    [Header("Movement Speeds")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 9f;
    [SerializeField] private float attackChaseSpeed = 6f; // Velocidad después del primer golpe
    [SerializeField] private float screamSpeed = 0f;

    [Header("Scream")]
    [SerializeField] private float screamDuration = 2f;

    [Header("Memory")]
    [SerializeField] private float alertDuration = 5f;

    [Header("Patrol Turning")]
    [SerializeField] private float patrolTurnThreshold = 15f;
    [SerializeField] private float patrolStopTime = 1f; // Tiempo que se queda quieta en el punto

    [Header("Attack")]
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float attackCooldown = 1.5f; // Tiempo entre ataques

    [Header("State")]
    [SerializeField] private EnemyState currentState;

    public EnemyState CurrentState => currentState;

    private int patrolIndex;
    private float alertTimer;
    private float screamTimer;
    private Vector3 lastKnownTargetPosition;
    private float patrolStopTimer;
    private bool hasHitOnce; // Controla si ya pegó el primer golpe
    private float attackCooldownTimer;

    public bool IsDead => currentState == EnemyState.Dead;

    //Animations
    public event Action OnAttackTriggered;
    public event Action OnScream;

    private void Start()
    {
        ChangeState(EnemyState.Patrol);
        patrolStopTimer = patrolStopTime;
    }

    private void Update()
    {
        EvaluateState();
        ExecuteState();

        enemyAnimator.UpdateSpeed(locomotion.currentSpeed, chaseSpeed);

    }

    public void Kill()
    {
        ChangeState(EnemyState.Dead);
    }

    private void EvaluateState()
    {
        if (currentState == EnemyState.Dead)
            return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                if (sensors.CanSeeTarget)
                {
                    lastKnownTargetPosition = sensors.Target.position;
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
                if (sensors.CanSeeTarget)
                {
                    lastKnownTargetPosition = sensors.Target.position;
                    alertTimer = alertDuration;

                    float distance = Vector3.Distance(transform.position, sensors.Target.position);

                    // Cuando llega a distancia de ataque
                    if (distance <= attackDistance && attackCooldownTimer <= 0f)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                }
                else
                {
                    ChangeState(EnemyState.Alert);
                }
                break;

            case EnemyState.Attack:
                attackCooldownTimer -= Time.deltaTime;

                if (attackCooldownTimer <= 0f)
                {
                    float distance = Vector3.Distance(transform.position, sensors.Target.position);

                    if (distance <= attackDistance && sensors.CanSeeTarget)
                    {
                        // Sigue atacando desde el lugar
                        attackCooldownTimer = attackCooldown;
                        OnAttackTriggered?.Invoke();
                    }
                    else
                    {
                        // El jugador se alejó, vuelve a perseguir
                        ChangeState(EnemyState.Chase);
                    }
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
                // Si ya pegó una vez, va más lento
                locomotion.SetMoveSpeed(hasHitOnce ? attackChaseSpeed : chaseSpeed);
                MoveTo(sensors.Target.position); // Persecución DIRECTA
                break;

            case EnemyState.Alert:
                locomotion.SetMoveSpeed(patrolSpeed);
                MoveTo(lastKnownTargetPosition);
                break;

            case EnemyState.Attack:
                HandleAttack();
                break;

            case EnemyState.Dead:
                HandleDead();
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

        PatrolPoint point = patrolPoints[patrolIndex];
        Vector3 direction = point.transform.position - transform.position;
        float distance = direction.magnitude; 

        Vector3 flatDirection = direction.normalized;
        float angle = Vector3.Angle(transform.forward, flatDirection);

        // Llegó al punto: se queda quieta
        if (distance <= patrolReachDistance)
        {
            locomotion.SetDesiredDirection(Vector3.zero);
            locomotion.SetMovementLocked(true);

            patrolStopTimer -= Time.deltaTime;

            // Después del tiempo de espera, avanza al siguiente punto
            if (patrolStopTimer <= 0f)
            {
                patrolIndex++;
                if (patrolIndex >= patrolPoints.Length)
                    patrolIndex = 0;

                patrolStopTimer = patrolStopTime;
                locomotion.SetMovementLocked(false); // IMPORTANTE: destraba
            }
            return;
        }

        // Si el ángulo es grande, solo gira
        if (angle > patrolTurnThreshold)
        {
            locomotion.SetMovementLocked(true);
            locomotion.SetDesiredDirection(flatDirection);
        }
        else
        {
            // Ya está mirando bien, ahora sí se mueve
            locomotion.SetMovementLocked(false);
            MoveTo(point.transform.position);
        }
    }

    private void HandleScream()
    {
        if (sensors.Target == null)
            return;

        Vector3 lookDirection = sensors.Target.position - transform.position;
        locomotion.SetDesiredDirection(Vector3.zero);
        locomotion.SetForcedLookDirection(lookDirection);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        locomotion.SetDesiredDirection(direction);
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        locomotion.ClearForcedLookDirection();
        locomotion.SetMovementLocked(false); // Siempre destraba al cambiar de estado

        switch (newState)
        {
            case EnemyState.Scream:
                screamTimer = screamDuration;
                TriggerScream();
                OnScream?.Invoke();
                break;

            case EnemyState.Patrol:
                hasHitOnce = false; // Resetea el flag si vuelve a patrullar
                patrolStopTimer = patrolStopTime;
                break;

            case EnemyState.Attack:
                attackCooldownTimer = attackCooldown;
                OnAttackTriggered?.Invoke();
                break;
        }
    }

    private void TriggerScream()
    {
        // Acá agregar tu audio/animación del grito
    }

    private void HandleAttack()
    {
        if (sensors.Target == null)
            return;

        Vector3 direction =
            sensors.Target.position - transform.position;

        locomotion.SetMoveSpeed(0f);

        locomotion.SetDesiredDirection(Vector3.zero);

        locomotion.SetForcedLookDirection(direction);

        attack.TryAttack();
    }

    private void HandleDead()
    {
        locomotion.SetDesiredDirection(Vector3.zero);
        locomotion.SetMovementLocked(true);
    }
}