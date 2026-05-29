using System;
using UnityEngine;

public class PiranhaIA : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySensors sensors;
    [SerializeField] private AquaticLocomotion locomotion;
    [SerializeField] private EnemyAttack attack;
    [SerializeField] private PiranhaAnimator piranhaAnimator;

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

    //Animations
    public event Action OnJumpscare;
    public event Action OnAttack;


    private void Update()
    {
        piranhaAnimator.UpdateSpeed(locomotion.currentSpeed);

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
        Debug.Log("Cambiando estado a: " + newState);
        currentState = newState;
        locomotion.ClearForcedLookDirection();

        switch (newState)
        {
            case State.Jumpscare:
                screamTimer = jumpscareScreamDuration;
                hasScreamed = false;
                OnJumpscare?.Invoke();
                break;

            case State.Attack:
                attackTimer = attackCooldown;
                OnAttack?.Invoke();
                break;
        }
    }

    private void TriggerJumpscare()
    {
        if (jumpscareSound != null)
        {
            AudioSource.PlayClipAtPoint(jumpscareSound, transform.position, 1f);
        }
    }
}