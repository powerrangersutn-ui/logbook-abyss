using UnityEngine;

public class PlayerStats: MonoBehaviour
{
    // desechable, para comprobar que hace daño.
    public float playerHealth = 10f;
    

    public void TakeDamage(float damage)
    {
        playerHealth -= damage;
        Debug.Log("Current Health " + playerHealth);

        if (playerHealth <= 0)
        {
            Debug.Log ("Player has died");
        }
    }
    
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Enemy"))
    //     {
    //         EnemyStats enemy = other.GetComponent<EnemyStats>();

    //         if (enemy != null)
    //         {
    //             enemy.EnemyTakeDamage(1);
    //         }
    //     }
    // }

}
