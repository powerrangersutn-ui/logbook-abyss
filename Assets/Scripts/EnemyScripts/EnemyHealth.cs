using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health;

    //Reducir vida (y otros efectos del daño)
    private void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Dead();
    }

    //que pasa cuando muere
    private void Dead()
    {
        Destroy(gameObject);
    }

  
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletScript bulletScript = collision.gameObject.GetComponent<BulletScript>();
            TakeDamage(bulletScript.damage);
            Destroy(collision.gameObject);
        }
    }
}
