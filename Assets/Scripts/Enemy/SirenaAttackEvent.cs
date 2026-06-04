using UnityEngine;

public class SirenaAttackEvent : MonoBehaviour
{
    [SerializeField] private EnemyAttack enemyAttack;

    // Unity llama esto desde el Animation Event
    public void OnAttackHit()
    {
        enemyAttack.TryAttack();
    }
}