using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth == null)
            Debug.LogError("PlayerHealth no encontrado en este objeto");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectamos si el trigger que nos tocó pertenece a un arma enemiga
        if (other.CompareTag("EnemyWeapon") || other.name.Contains("arma"))
        {
            EnemyWeapon enemyWeapon = other.GetComponent<EnemyWeapon>();
            if (enemyWeapon == null)
                enemyWeapon = other.GetComponentInParent<EnemyWeapon>();

            if (enemyWeapon != null)
            {
                Debug.Log($"[PlayerDamageReceiver] Recibido ataque enemigo - Daño: {enemyWeapon.damage}");
                playerHealth.TakeDamage(enemyWeapon.damage);
            }
        }
    }
}