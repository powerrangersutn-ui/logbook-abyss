// MeleeAttackModule.cs - Ataque melee modular
using UnityEngine;

public class MeleeAttackModule : EnemyModule
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackAngle = 60f; // Ángulo del cono de ataque

    [Header("Attack Behavior")]
    [SerializeField] private bool rushToTarget = true;
    [SerializeField] private float rushSpeedMultiplier = 1.5f;
    [SerializeField] private float optimalAttackDistance = 1.5f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem attackEffect;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float attackEffectDuration = 0.3f;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private MovementModule movementModule;

    protected override void OnInitialize()
    {
        movementModule = core.GetModule<MovementModule>();
    }

    public override void OnUpdate()
    {
        if (!isActive) return;

        attackTimer -= Time.deltaTime;

        if (!sharedData.HasTarget) return;

        float distanceToTarget = sharedData.DistanceToTarget;

        // Si está en rango de ataque
        if (distanceToTarget <= attackRange)
        {
            // Ajustar posición para distancia óptima
            if (distanceToTarget > optimalAttackDistance && movementModule != null)
            {
                movementModule.MoveToPosition(sharedData.currentTarget.position, rushToTarget ? rushSpeedMultiplier : 1f);
            }
            else if (distanceToTarget < optimalAttackDistance * 0.7f && movementModule != null)
            {
                // Retroceder si está demasiado cerca
                Vector3 retreatDirection = (transform.position - sharedData.currentTarget.position).normalized;
                Vector3 retreatPosition = transform.position + retreatDirection * 2f;
                movementModule.MoveToPosition(retreatPosition, 0.5f);
            }

            // Atacar si puede
            if (attackTimer <= 0 && !isAttacking)
            {
                PerformAttack();
            }
        }
        // Si está cerca pero fuera de rango, perseguir
        else if (rushToTarget && movementModule != null)
        {
            movementModule.MoveToPosition(sharedData.currentTarget.position, rushSpeedMultiplier);
        }
    }

    private void PerformAttack()
    {
        if (!sharedData.HasTarget) return;

        // Verificar si el objetivo está en el cono de ataque
        Vector3 directionToTarget = (sharedData.currentTarget.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);

        if (angle > attackAngle / 2f)
        {
            // Fuera del cono, no atacar aún
            return;
        }

        isAttacking = true;
        attackTimer = attackCooldown;

        // Aplicar daño
        var targetHealth = sharedData.currentTarget.GetComponent<PlayerHealth>();
        if (targetHealth != null)
        {
            Vector3 hitDirection = directionToTarget;
            targetHealth.TakeDamage(attackDamage);
        }

        // Efectos visuales
        if (attackEffect != null)
        {
            attackEffect.Play();
        }

        // Efecto de sonido
        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }

        // Actualizar SharedData
        sharedData.lastAttackTime = Time.time;

        // Desactivar flag de ataque
        Invoke(nameof(ResetAttackFlag), attackEffectDuration);
    }

    private void ResetAttackFlag()
    {
        isAttacking = false;
    }

    public bool CanAttack()
    {
        return attackTimer <= 0 && !isAttacking;
    }

    public bool IsInAttackRange()
    {
        return sharedData.HasTarget && sharedData.DistanceToTarget <= attackRange;
    }

    public override void DrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibujar cono de ataque
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Vector3 leftBoundary = Quaternion.Euler(0, -attackAngle / 2f, 0) * transform.forward * attackRange;
            Vector3 rightBoundary = Quaternion.Euler(0, attackAngle / 2f, 0) * transform.forward * attackRange;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        }
    }
}