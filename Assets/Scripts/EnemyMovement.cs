using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Timeline;

public class EnemyMovementTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float minDistance = 1.8f;
    [SerializeField] private float maxDistance = 8f;

    
    

    
        

    void Start()
    {
        
    }

    void Update()
    {
        float distanciaActual = Vector3.Distance(transform.position, player.position);

        if (distanciaActual > minDistance && distanciaActual < maxDistance)
        {       
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            transform.LookAt(player.position);
        }
        else if (distanciaActual <= minDistance)
        {
            Attack();
        }
        

    }   
    private void Attack()
    {
        Debug.Log("Atacando");
    }
}
