using System.Collections;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [Header("Configuración del Arma")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] public int damage = 10;
    [SerializeField] private float attackDuration = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void Awake()
    {
        ValidateCollider();
    }

    private void Start()
    {
        Debug.Log($"EnemyWeapon Start - Este script esta en: {gameObject.name}");
        Debug.Log($"weaponCollider asignado: {weaponCollider != null}");
        if (weaponCollider != null)
        {
            Debug.Log($"weaponCollider esta en el objeto: {weaponCollider.gameObject.name}");
            Debug.Log($"weaponCollider.isTrigger: {weaponCollider.isTrigger}");
            Debug.Log($"weaponCollider.enabled: {weaponCollider.enabled}");
        }
    }

    private void ValidateCollider()
    {
        if (weaponCollider == null)
        {
            Debug.LogError($"[{gameObject.name}] NO SE ASIGNO el Weapon Collider en el Inspector", this);
            return;
        }

        if (!weaponCollider.isTrigger)
        {
            Debug.LogWarning($"[{gameObject.name}] El collider del arma NO es Trigger. Cambiando automaticamente");
            weaponCollider.isTrigger = true;
        }

        weaponCollider.enabled = false;

        if (showDebugLogs)
            Debug.Log($"[{gameObject.name}] EnemyWeapon validado - Daño: {damage}");
    }

    public IEnumerator ActivateAttack()
    {
        Debug.Log($"========== ACTIVANDO ATAQUE ==========");
        Debug.Log($"GameObject que ejecuta: {gameObject.name}");

        if (weaponCollider == null)
        {
            Debug.LogError($"[{gameObject.name}] weaponCollider es NULL - No se asigno en el Inspector");
            yield break;
        }

        weaponCollider.enabled = true;
        Debug.Log($"[{gameObject.name}] Hitbox ACTIVADA - Collider enabled: {weaponCollider.enabled}");
        Debug.Log($"Posicion del arma: {weaponCollider.transform.position}");

        yield return new WaitForSeconds(attackDuration);

        weaponCollider.enabled = false;
        Debug.Log($"[{gameObject.name}] Hitbox DESACTIVADA");
        Debug.Log($"========================================");
    }

    public void StartAttack()
    {
        Debug.Log($"StartAttack llamado desde: {gameObject.name}");
        StartCoroutine(ActivateAttack());
    }
}