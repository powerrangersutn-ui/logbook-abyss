using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int damage = 1;

    [SerializeField] private float attackRange = 2f;

    [SerializeField] private float attackCooldown = 1.5f;

    [SerializeField] private LayerMask playerMask;

    [Header("References")]
    [SerializeField] private Transform attackPoint;

    private float cooldownTimer;

    public bool CanAttack => cooldownTimer <= 0f;

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public bool TryAttack()
    {
        if (!CanAttack)
            return false;

        Collider[] hits =
            Physics.OverlapSphere(
                attackPoint.position,
                attackRange,
                playerMask);

        foreach (Collider hit in hits)
        {
            PlayerHealth player =
                hit.GetComponent<PlayerHealth>();

            if (player == null)
                continue;

            if (player.IsDead)
                continue;
            Debug.Log("HIT PLAYER");
            player.TakeDamage(damage);

            cooldownTimer = attackCooldown;

            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange);
    }
}