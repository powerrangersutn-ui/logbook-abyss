using UnityEngine;

public class PiranhaAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySensors sensors;
    [SerializeField] private AquaticLocomotion locomotion;
    [SerializeField] private EnemyAttack attack;

    [Header("Behavior")]
    [SerializeField] private float detectionRange = 8f; // Rango para activarse
    [SerializeField] private float chaseSpeed = 12f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float desiredCombatDistance = 1.2f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Jumpscare")]
    [SerializeField] private float jumpscareScreamDuration = 0.3f;
    [SerializeField] private AudioClip jumpscareSound;

    private enum State { Idle, Jumpscare, Chase, Attack }
    private State currentState = State.Idle;

    private float attackTimer;
    private float screamTimer;
    private bool hasScreamed;

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdle();
                break;

            case State.Jumpscare:
                HandleJumpscare();
                break;

            case State.Chase:
                HandleChase();
                break;

            case State.Attack:
                HandleAttack();
                break;
        }
    }

    private void HandleIdle()
    {
        locomotion.SetDesiredDirection(Vector3.zero);

        if (sensors.Target == null)
            return;

        float distance = Vector3.Distance(transform.position, sensors.Target.position);

        if (distance <= detectionRange)
        {
            ChangeState(State.Jumpscare);
        }
    }

    private void HandleJumpscare()
    {
        screamTimer -= Time.deltaTime;

        Vector3 lookDir = sensors.Target.position - transform.position;
        locomotion.SetDesiredDirection(Vector3.zero);
        locomotion.SetForcedLookDirection(lookDir);

        if (!hasScreamed)
        {
            TriggerJumpscare();
            hasScreamed = true;
        }

        if (screamTimer <= 0f)
        {
            ChangeState(State.Chase);
        }
    }

    private void HandleChase()
    {
        if (sensors.Target == null)
            return;

        locomotion.SetMoveSpeed(chaseSpeed);

        float distance = Vector3.Distance(transform.position, sensors.Target.position);

        if (distance <= attackDistance && attackTimer <= 0f)
        {
            ChangeState(State.Attack);
            return;
        }

        Vector3 direction = sensors.Target.position - transform.position;
        locomotion.SetDesiredDirection(direction);
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if (sensors.Target == null)
            return;

        Vector3 direction =
            sensors.Target.position - transform.position;

        float distance =
            direction.magnitude;

        locomotion.SetForcedLookDirection(direction);

        if (distance > desiredCombatDistance)
        {
            locomotion.SetDesiredDirection(direction);
        }
        else
        {
            locomotion.SetDesiredDirection(Vector3.zero);
        }

        if (attackTimer <= 0f)
        {
            attack.TryAttack();

            ChangeState(State.Chase);
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        locomotion.ClearForcedLookDirection();

        switch (newState)
        {
            case State.Jumpscare:
                screamTimer = jumpscareScreamDuration;
                hasScreamed = false;
                break;

            case State.Attack:
                attackTimer = attackCooldown;
                break;
        }
    }

    private void TriggerJumpscare()
    {
        Debug.Log("PIRAÑA JUMPSCARE");

        if (jumpscareSound != null)
        {
            AudioSource.PlayClipAtPoint(jumpscareSound, transform.position, 1f);
        }
    }
}