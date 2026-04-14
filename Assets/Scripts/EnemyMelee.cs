using System.Collections;
//using UnityEditor.Rendering;
using UnityEngine;
//using UnityEngine.InputSystem.LowLevel;
//using UnityEngine.Timeline;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private GameObject enemyAttack;
    [SerializeField] private float damage = 1f;
    

    void Start()
    {
    }

    
    void Update()
    {
        
    }

    public void EnemyMelee()
    {
        StartCoroutine(ActivateAttack());
    }

    // Disable the collider so the weapon doesnt hit continuously
    public IEnumerator ActivateAttack()
    {
        Debug.Log("Hitbox activado");
        gameObject.SetActive(true);
        enemyAttack.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        enemyAttack.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("Hitbox desactivado");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisiona con: " + other.name);
        
        if (other.CompareTag("Player"))
        {   
            Debug.Log("Golpea al player");

            PlayerStats player = other.GetComponent<PlayerStats>();

            if (player != null)
            {
                player.TakeDamage((int)damage);
            }
        }
    }
}
