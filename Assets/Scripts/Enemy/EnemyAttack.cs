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

        Debug.Log($"=== EnemyAttack Awake en {gameObject.name} ===");
        Debug.Log($"EnemyWeapon encontrado: {enemyWeapon != null}");
        if (enemyWeapon != null)
        {
            Debug.Log($"EnemyWeapon esta en: {enemyWeapon.gameObject.name}");
        }
    }

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
        Debug.Log($"Player asignado en EnemyAttack: {player != null}");
        if (player != null)
        {
            Debug.Log($"Player es: {player.name}");
        }
    }

    public void TryAttack()
    {
        Debug.Log($"TryAttack llamado en {gameObject.name}");

        if (player == null)
        {
            Debug.LogWarning("Player es NULL - no se puede atacar");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log($"Distancia al player: {distance} | Attack Range: {attackRange}");

        float timeSinceLastAttack = Time.time - lastAttackTime;
        Debug.Log($"Tiempo desde ultimo ataque: {timeSinceLastAttack} | Cooldown: {attackCooldown}");

        if (distance <= attackRange && timeSinceLastAttack >= attackCooldown)
        {
            Debug.Log("CONDICIONES CUMPLIDAS - Ejecutando ataque");
            PerformAttack();
        }
        else
        {
            Debug.Log("Condiciones NO cumplidas para atacar");
        }
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;
        IsAttacking = true;

        Debug.Log($"========== PERFORM ATTACK ==========");
        Debug.Log($"Enemigo: {gameObject.name}");

        if (enemyWeapon != null)
        {
            Debug.Log("Iniciando ActivateWeaponAttack coroutine");
            StartCoroutine(ActivateWeaponAttack());
        }
        else
        {
            Debug.LogError("EnemyWeapon es NULL - No se puede atacar");
            IsAttacking = false;
        }
    }

    private IEnumerator ActivateWeaponAttack()
    {
        Debug.Log("ActivateWeaponAttack coroutine iniciada");

        if (enemyWeapon == null)
        {
            Debug.LogError("enemyWeapon es NULL en la coroutine");
            yield break;
        }

        yield return StartCoroutine(enemyWeapon.ActivateAttack());

        IsAttacking = false;
        Debug.Log("ActivateWeaponAttack coroutine completada");
        Debug.Log($"====================================");
    }

    public void StopAttacking()
    {
        IsAttacking = false;
        StopAllCoroutines();
    }

    public void SetAttackCooldown(float newCooldown)
    {
        if (newCooldown > 0)
            attackCooldown = newCooldown;
    }
}