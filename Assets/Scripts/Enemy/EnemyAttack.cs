using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackRange = 2.0f;

    private Transform player;
    private EnemyWeapon enemyWeapon;
    private float lastAttackTime;

    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        enemyWeapon = GetComponentInChildren<EnemyWeapon>(true);
    }

    /// <summary>
    /// Inicializa el sistema de ataque con la referencia al player
    /// </summary>
    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    /// <summary>
    /// Llamado por el movimiento cuando el enemigo está en rango de ataque
    /// </summary>
    public void TryAttack()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        IsAttacking = true;

        Debug.Log($"[{gameObject.name}] ¡Ataque ejecutado!");

        if (enemyWeapon != null)
        {
            StartCoroutine(ActivateWeaponAttack());
        }
        else
        {
            Debug.LogWarning("No se encontró EnemyWeapon en el enemigo", this);
            IsAttacking = false;
        }
    }

    private IEnumerator ActivateWeaponAttack()
    {
        // Aquí podrías agregar lógica extra antes de activar el arma (animaciones, etc.)
        yield return StartCoroutine(enemyWeapon.ActivateAttack());

        // Cuando termina el ataque del arma, terminamos el estado de attacking
        IsAttacking = false;
    }

    /// <summary>
    /// Llamado cuando el enemigo muere o se desactiva
    /// </summary>
    public void StopAttacking()
    {
        IsAttacking = false;
        StopAllCoroutines(); // por si había un ataque en curso
    }

    // Opcional: Método para cambiar el cooldown desde afuera si lo necesitás
    public void SetAttackCooldown(float newCooldown)
    {
        if (newCooldown > 0)
            attackCooldown = newCooldown;
    }
}