using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float enemyHealth = 2f;

    public void EnemyTakeDamage(float damage)
    {
        Debug.Log("Enemy Health " + enemyHealth);
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy Died");
        Destroy(gameObject);
    }
}
