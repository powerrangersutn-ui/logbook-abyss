using System.Collections;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackDuration = 0.3f;

    private Collider hitboxCollider;

    private void Awake()
    {
        // Busca el collider en el hijo "arma"
        Transform arma = transform.Find("arma");
        if (arma != null)
        {
            hitboxCollider = arma.GetComponent<Collider>();
            if (hitboxCollider != null)
            {
                hitboxCollider.isTrigger = true;
                hitboxCollider.enabled = false;
                Debug.Log("✅ EnemyWeapon: Collider del arma listo");
            }
            else
            {
                Debug.LogError("❌ No se encontró Collider en el hijo 'arma'", arma);
            }
        }
    }

    public IEnumerator ActivateAttack()
    {
        if (hitboxCollider == null)
        {
            Debug.LogWarning("No hay hitboxCollider");
            yield break;
        }

        hitboxCollider.enabled = true;
        Debug.Log("Hitbox ACTIVADO");

        yield return new WaitForSeconds(attackDuration);

        hitboxCollider.enabled = false;
        Debug.Log("Hitbox DESACTIVADO");
    }

    // OnTriggerEnter en el padre
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("¡DAÑO APLICADO AL JUGADOR! (desde EnemyWeapon)");
            }
        }
    }
}