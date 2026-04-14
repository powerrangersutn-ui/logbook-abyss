using System.Collections;
//using UnityEditor.Rendering;
using UnityEngine;
//using UnityEngine.InputSystem.LowLevel;
//using UnityEngine.Timeline;

public class EnemyWeapon : MonoBehaviour
{
    
    void Start()
    {
    }

    
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("EnemyAttack"))
        {
            print("Daño");
        }
    }
}
